using System.Net;
using System.Net.Mail;
using System.Reflection;
using ClosedXML.Excel;
using EPAY.AIRWAY.KIOSK.API.Domain.Context;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Extensions.AddConfig;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Report.Response;
using EPAY.AIRWAY.KIOSK.API.Resources.Exceptions;
using EPAY.AIRWAY.KIOSK.API.Resources.SystemData.CronJob.Report;
using Microsoft.EntityFrameworkCore;

namespace EPAY.AIRWAY.KIOSK.API.Services.CronJob;

/// <summary>
/// Job gửi báo cáo giao dịch qua email
/// </summary>
public sealed class PaymentReportJob : CronJobService
{
    #region Properties

    private readonly IServiceProvider _serviceProvider;

    #endregion

    #region Constructor

    public PaymentReportJob(IServiceProvider serviceProvider,
        IScheduleConfig<PaymentReportJob> config) : base(config.CronExpression, config.TimeZoneInfo)
    {
        _serviceProvider = serviceProvider;
    }

    #endregion

    #region Method

    protected override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        string jobId = string.Empty;

        try
        {
            await Task.Delay(Random.Shared.Next(1000, 9999), cancellationToken);
            jobId = RelateText.GenId();

            /*
            1) Đối với báo cáo ngày, job sẽ lấy thời gian hiện tại làm mốc và lùi một ngày
            1) Đối với báo cáo tháng, job sẽ chạy vào ngày 1 của tháng sau
            */

            DateTime now = DateTime.UtcNow.ConvertUtcToVietnamTz();

            if (now.Day == 1) // Xử lí lấy báo cáo tháng
                await ProcessPaymentMonthlyReportAsync(now.AddDays(-1), jobId, cancellationToken);

            // Xử lí lấy báo cáo ngày
            await ProcessPaymentDailyReportAsync(now.AddDays(-1), jobId, cancellationToken);
        }
        catch (Exception ex)
        {
            JobContext.LogWithContext().Error($"{nameof(PaymentReportJob)} ({jobId}) is fail: {ex.Message}", ex);
        }
    }

    public async Task ProcessPaymentDailyReportAsync(DateTime date, string jobId, CancellationToken cancellationToken = default)
    {
        try
        {
            JobContext.LogWithContext().Information($"{nameof(PaymentReportJob)} ({jobId}) is working with date: {date:dd/MM/yyyy}");

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CoreContext>();

            // Kiểm tra đã có job trước đó thực hiện tác vụ này chưa.
            var value = await context.CronJobFlags.FirstOrDefaultAsync(x => x.Name == $"{nameof(PaymentReportJob)}/Daily", cancellationToken);
            if (value != null)
            {
                // Không thực hiện lại tác vụ nếu đã thực hiện trước đó trong khoảng 5 phút
                var utcNow = DateTime.UtcNow;
                if (utcNow.Subtract(value.UpdatedDatetimeUtc).TotalSeconds <= (5 * 60))
                {
                    JobContext.LogWithContext().Information($"{nameof(PaymentReportJob)} ({jobId}) is cancel");
                    return;
                }

                value.UpdatedDatetimeUtc = utcNow;
                context.CronJobFlags.Update(value);
                await context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                JobContext.LogWithContext().Information($"{nameof(PaymentReportJob)} ({jobId}) is cancel");
                return;
            }

            var configurationService = scope.ServiceProvider.GetRequiredService<IConfigurationService>();
            var flightService = scope.ServiceProvider.GetRequiredService<IFlightService>();
            var emailConfig = await GetConfigDataAsync(configurationService, cancellationToken);
            var masterData = await flightService.GetMasterDataAsync(false, cancellationToken);

            // Lấy thông tin report
            // Convert localTime -> UtcTime
            DateTime utcTimeOne = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0).ConvertVietnamTzToUtc();
            DateTime utcTimeTwo = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999).ConvertVietnamTzToUtc();
            var reports = await context.Reports
                .AsNoTracking()
                .Where(x => DateTime.Compare(x.CreatedDatetimeUtc, utcTimeOne) >= 0 && DateTime.Compare(x.CreatedDatetimeUtc, utcTimeTwo) <= 0)
                .ToListAsync(cancellationToken);

            // Lấy thông tin sale-channel
            var salesChannel = await context.SalesChannel
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // Lấy thông tin service-partners
            var servicePartners = await context.ServicePartners
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // Lấy thông tin locations
            var locations = await context.Locations
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var source = MappingData(reports, salesChannel, servicePartners, locations, masterData.Data);

            // Xử lí file excel
            using MemoryStream memStr = new();
            ProcessExcelFile(source, memStr);

            // Xử lí gửi mail
            memStr.Position = 0;
            emailConfig.Subject = $"[EHF_Airline] Danh sách giao dịch thanh toán mua vé máy bay từ EpayHostedForm Airline ngày {date:dd/MM/yyyy}";
            emailConfig.Body = $"Kính gửi phòng Đối soát,\r\n\r\n" +
                               $"Hệ thống EpayHostedForm Airline gửi danh sách giao dịch thanh toán mua vé máy bay ngày {date:dd/MM/yyyy} tại tệp đính kèm.\r\n" +
                               $"Trân trọng,\r\n" +
                               $"Hệ thống EpayHostedForm Airline";
            emailConfig.FileName = $"{date:yyyy}_{date:MM}_{date:dd}_Airline_Sales.xlsx";

            await ProcessMailAsync(emailConfig, memStr, cancellationToken);
        }
        catch (Exception ex)
        {
            if (ex is DbUpdateConcurrencyException)
                JobContext.LogWithContext().Information($"{nameof(PaymentReportJob)} ({jobId}) is cancel");
            else
                JobContext.LogWithContext().Error($"{nameof(PaymentReportJob)} ({jobId}) is fail: {ex.Message}", ex);

            throw;
        }
    }

    public async Task ProcessPaymentMonthlyReportAsync(DateTime date, string jobId, CancellationToken cancellationToken = default)
    {
        try
        {
            JobContext.LogWithContext().Information($"{nameof(PaymentReportJob)} ({jobId}) is working with month: {date:MM/yyyy}");

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CoreContext>();
            var configurationService = scope.ServiceProvider.GetRequiredService<IConfigurationService>();
            var flightService = scope.ServiceProvider.GetRequiredService<IFlightService>();

            // Kiểm tra đã có job trước đó thực hiện tác vụ này chưa.
            var value = await context.CronJobFlags.FirstOrDefaultAsync(x => x.Name == $"{nameof(PaymentReportJob)}/Monthly", cancellationToken);
            if (value != null)
            {
                // Không thực hiện lại tác vụ nếu đã thực hiện trước đó trong khoảng 5 phút
                var utcNow = DateTime.UtcNow;
                if (utcNow.Subtract(value.UpdatedDatetimeUtc).TotalSeconds <= (5 * 60))
                {
                    JobContext.LogWithContext().Information($"{nameof(PaymentReportJob)} ({jobId}) is cancel");
                    return;
                }

                value.UpdatedDatetimeUtc = utcNow;
                context.CronJobFlags.Update(value);
                await context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                JobContext.LogWithContext().Information($"{nameof(PaymentReportJob)} ({jobId}) is cancel");
                return;
            }

            var emailConfig = await GetConfigDataAsync(configurationService, cancellationToken);
            var masterData = await flightService.GetMasterDataAsync(false, cancellationToken);

            // Lấy thông tin report
            // Convert localTime -> UtcTime
            // Convert localTime -> UtcTime
            DateTime utcTimeOne = new DateTime(date.Year, date.Month, 1, 0, 0, 0, 0).ConvertVietnamTzToUtc();
            DateTime utcTimeTwo = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 23, 59, 59, 999).ConvertVietnamTzToUtc();
            var reports = await context.Reports
                .AsNoTracking()
                .Where(x => DateTime.Compare(x.CreatedDatetimeUtc, utcTimeOne) >= 0 && DateTime.Compare(x.CreatedDatetimeUtc, utcTimeTwo) <= 0)
                .ToListAsync(cancellationToken);

            // Lấy thông tin sale-channel
            var salesChannel = await context.SalesChannel
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // Lấy thông tin service-partners
            var servicePartners = await context.ServicePartners
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // Lấy thông tin locations
            var locations = await context.Locations
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var source = MappingData(reports, salesChannel, servicePartners, locations, masterData.Data);

            // Xử lí file excel
            using MemoryStream memStr = new();
            ProcessExcelFile(source, memStr);

            // Xử lí gửi mail
            memStr.Position = 0;
            emailConfig.Subject = $"[EHF_Airline] Danh sách giao dịch thanh toán mua vé máy bay từ EpayHostedForm Airline tháng {date:MM/yyyy}";
            emailConfig.Body = $"Kính gửi phòng Đối soát,\r\n\r\n" +
                               $"Hệ thống EpayHostedForm Airline gửi danh sách giao dịch thanh toán mua vé máy bay tháng {date:MM/yyyy} tại tệp đính kèm.\r\n" +
                               $"Trân trọng,\r\n" +
                               $"Hệ thống EpayHostedForm Airline";
            emailConfig.FileName = $"{date:yyyy}_{date:MM}_Airline_Sales.xlsx";

            await ProcessMailAsync(emailConfig, memStr, cancellationToken);
        }
        catch (Exception ex)
        {
            if (ex is DbUpdateConcurrencyException)
                JobContext.LogWithContext().Information($"{nameof(PaymentReportJob)} ({jobId}) is cancel");
            else
                JobContext.LogWithContext().Error($"{nameof(PaymentReportJob)} ({jobId}) is fail: {ex.Message}", ex);

            throw;
        }
    }

    /// <summary>
    /// Chức năng: kiểm tra chức năng gửi email
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async Task TestConnectAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var configurationService = scope.ServiceProvider.GetRequiredService<IConfigurationService>();

            var emailConfig = await GetConfigDataAsync(configurationService, cancellationToken);

            // Xử lí gửi mail
            emailConfig.AddressTo = emailConfig.TestAddressTo;
            emailConfig.Subject = $"[EHF_Airline] Kiểm tra kết nối dịch vụ";
            emailConfig.Body = "Đây là email kiểm tra kết nối dịch vụ, vui lòng bỏ qua thông tin này!";

            await ProcessMailAsync(emailConfig, null, cancellationToken);
        }
        catch (Exception ex)
        {
            JobContext.LogWithContext().Error($"{nameof(PaymentReportJob)}/{nameof(TestConnectAsync)} is fail: {ex.Message}", ex);
            throw;
        }
    }

    #region Private work

    private static TemplatePaymentResponse MappingData(List<Model.ReportSection.Report>? report,
        List<Model.ReportSection.SaleChannel>? salesChannel,
        List<Model.ReportSection.ServicePartner>? servicePartners,
        List<Model.ReportSection.Location>? locations,
        MasterDataResponse? masterData)
    {
        string formatDatetime = "dd/MM/yyyy HH:mm:ss";

        TemplatePaymentResponse result = new()
        {
            Report = new()
        };

        if (report == null || report.Count <= 0)
            return result;

        foreach (var item in report)
        {
            if (item?.OtherInfo?.ListFareData == null || item.OtherInfo.ListFareData.Count <= 0)
                break;

            foreach (var fare in item.OtherInfo.ListFareData)
            {
                result.Report.Add(new()
                {
                    SaleChannel = salesChannel?.FirstOrDefault(x => x.Code == item?.SaleChannelCode)?.Name,
                    ServicePartner = servicePartners?.FirstOrDefault(x => x.Code == item?.ServicePartnerCode)?.Name,
                    Location = locations?.FirstOrDefault(x => x.Code == item?.LocationCode)?.Name,
                    DeviceSerial = item?.DeviceSerial,
                    CreatePaymentDatetime = item?.CreatedDatetimeUtc.ConvertUtcToVietnamTz().ToString(formatDatetime),
                    OrderCode = item?.OrderCode,
                    AbTripOrderId = item?.OtherInfo?.OrderId,
                    Route = MappingRouteData(item, masterData, fare.IsDeparture),
                    RouteType = MappingRouteTypeData(item),
                    TicketType = MappingTicketTypeData(item),
                    TicketQuantityAdt = fare.TicketQuantityAdt,
                    TicketNumberAdt = fare.TicketNumberAdt,
                    TicketQuantityChd = fare.TicketQuantityChd,
                    TicketNumberChd = fare.TicketNumberChd,
                    TotalPrice = fare.TotalPrice,
                    PaymentType = MappingPaymentTypeData(item),
                    PaymentProviderStatus = MappingPaymentProviderStatusData(item),
                    ServiceProviderStatus = MappingServiceProviderStatusData(fare.ServiceProviderStatus),
                    Note = MappingNoteData(fare)
                });
            }
        }

        // Sắp xếp theo thời gian tăng dần
        result.Report = result.Report.OrderBy(x => x.CreatePaymentDatetime).ThenBy(x => x.OrderCode).ToList();

        return result;
    }

    private static string? MappingNoteData(Model.ReportSection.ToJson.FareData? fareData)
    {
        if (fareData == null)
            return string.Empty;

        List<string> result = new();
        if (fareData.ListBaggage != null && fareData.ListBaggage.Count > 0)
            foreach (var baggage in fareData.ListBaggage)
                result.Add($"{baggage.Name} {baggage.Price}");

        if (fareData.ListAncillary != null && fareData.ListAncillary.Count > 0)
            foreach (var ancillary in fareData.ListAncillary)
                result.Add($"{ancillary.Name} {ancillary.Price}");

        return string.Join(", ", result);
    }

    private static string? MappingServiceProviderStatusData(bool? serviceProviderStatus)
    {
        return serviceProviderStatus == true ? "Thành công" : "Thất bại";
    }

    private static string? MappingPaymentProviderStatusData(Model.ReportSection.Report? report)
    {
        if (report?.PartnerPaymentStatus == "1")
            return "Thành công";

        return "Thất bại";
    }

    private static string? MappingPaymentTypeData(Model.ReportSection.Report? report)
    {
        if (string.IsNullOrEmpty(report?.PaymentType))
            return string.Empty;

        // Xử lí với partner-payment-type
        switch (report.PartnerPaymentType)
        {
            case "1":
            case "01":
                return "Ví EPAY";
            case "2":
            case "02":
                return "Thẻ nội địa";
            case "3":
            case "03":
                return "Thẻ thẻ quốc tế";
            case "4":
            case "04":
                return "Mã QR";
        }

        // Xử lí với payment-type
        switch (report.PaymentType)
        {
            case "1":
                return "Mã QR";
            case "2":
                return "Ví EPAY";
            case "3":
                return "POS";
            case "4":
                return "Tiền mặt";
            case "5":
                return "Thẻ nội địa";
            case "6":
                return "Thẻ quốc tế";
            case "7":
                return "Tài khoản ngân hàng";
        }

        return string.Empty;
    }

    private static string? MappingTicketTypeData(Model.ReportSection.Report? report)
    {
        if (string.IsNullOrEmpty(report?.OtherInfo?.TicketType))
            return string.Empty;

        if (report?.OtherInfo?.TicketType == "1")
            return "Một chiều";

        if (report?.OtherInfo?.TicketType == "2")
            return "Khứ hồi";

        return string.Empty;
    }

    private static string? MappingRouteTypeData(Model.ReportSection.Report? report)
    {
        if (string.IsNullOrEmpty(report?.OtherInfo?.JourneyType))
            return string.Empty;

        if (report?.OtherInfo?.JourneyType == "1")
            return "Nội địa";

        if (report?.OtherInfo?.JourneyType == "2")
            return "Quốc tế";

        return string.Empty;
    }

    private static string? MappingRouteData(Model.ReportSection.Report? report, MasterDataResponse? masterData, bool? type)
    {
        if (report == null || masterData == null)
            return string.Empty;

        string? startPoint = masterData?.Airports?.Find(x => x.Code.Equals(report?.OtherInfo?.StartPoint))?.CityName;
        string? endPoint = masterData?.Airports?.Find(x => x.Code.Equals(report?.OtherInfo?.EndPoint))?.CityName;

        // Với trường hợp quốc tế khứ hồi
        if (report?.OtherInfo?.JourneyType == "2" && report.OtherInfo.TicketType == "2")
            return $"{startPoint} - {endPoint}\r\n{endPoint} - {startPoint}";

        return type == true ? $"{startPoint} - {endPoint}" : $"{endPoint} - {startPoint}";
    }

    /// <summary>
    /// Chức năng: tạo file báo cáo
    /// </summary>
    /// <param name="source"></param>
    /// <param name="str"></param>
    /// <exception cref="ArgumentNullException"></exception>
    private void ProcessExcelFile(TemplatePaymentResponse source, Stream str)
    {
        if (source is null || str is null)
            throw new ArgumentNullException();

        bool isValid = source.Report != null && source.Report.Count > 0;

        using var wbook = new XLWorkbook();
        var ws = wbook.AddWorksheet("Report");

        // Set title
        var sourceProperties = typeof(TemplatePaymentInner).GetProperties();
        foreach (var prop in sourceProperties)
        {
            var jsonPropertyOrderAttribute = prop.GetCustomAttribute<JsonPropertyOrderAttribute>();
            var jsonPropertyNameAttribute = prop.GetCustomAttribute<JsonPropertyNameAttribute>();
            if (jsonPropertyOrderAttribute != null && jsonPropertyNameAttribute != null)
            {
                int order = jsonPropertyOrderAttribute.Order;

                var title = ws.Cell(1, order);
                title.SetValue(jsonPropertyNameAttribute.Name).SetActive();
                title.Style.Font.Bold = true;
                title.Style.Fill.SetBackgroundColor(XLColor.PeachPuff);
            }
        }

        // Gán dữ liệu vào cell
        if (isValid)
        {
            int lastCellColumn = 0;

            int reportCount = source!.Report!.Count;
            for (int i = 0; i < reportCount; i++)
            {
                int row = i + 2;

                var reportProperties = source!.Report[i].GetType().GetProperties();
                foreach (var prop in reportProperties)
                {
                    var jsonPropertyOrderAttribute = prop.GetCustomAttribute<JsonPropertyOrderAttribute>();
                    if (jsonPropertyOrderAttribute != null)
                    {
                        int order = lastCellColumn = jsonPropertyOrderAttribute.Order;

                        if (order == 1)
                        {
                            ws.Cell(row, 1).Value = i + 1;
                            continue;
                        }

                        var stringNumeric = prop.GetValue(source?.Report[i])?.ToString();
                        if (Nullable.GetUnderlyingType(prop.PropertyType) == typeof(long) && long.TryParse(stringNumeric, out long longParsed))
                        {
                            ws.Cell(row, order).Value = longParsed;
                            ws.Cell(row, order).Style.NumberFormat.NumberFormatId = (int)XLPredefinedFormat.Number.IntegerWithSeparator;
                        }
                        else
                            ws.Cell(row, order).Value = stringNumeric;
                    }
                }
            }

            // Apply style for worksheet
            ws.Range(1, 1, reportCount + 1, lastCellColumn).Style
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Border.SetInsideBorder(XLBorderStyleValues.Thin)
                .Alignment.SetWrapText(true)
                .Alignment.SetVertical(XLAlignmentVerticalValues.Top);
        }

        wbook.SaveAs(str);
    }

    /// <summary>
    /// Chức năng: sử dụng smtp gởi email
    /// </summary>
    /// <param name="request"></param>
    /// <param name="str"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="ArgumentNullException"></exception>
    private async Task ProcessMailAsync(EmailConfig request, Stream? str, CancellationToken cancellationToken = default)
    {
        MailMessage email = new MailMessage();

        // From
        email.From = new MailAddress(request.AddressFrom ?? throw new ArgumentNullException());

        // To
        if (!string.IsNullOrEmpty(request.AddressTo))
            foreach (var to in request.AddressTo.Split(';', StringSplitOptions.RemoveEmptyEntries))
                email.To.Add(to);

        // CC
        if (!string.IsNullOrEmpty(request.AddressCC))
            foreach (var cc in request.AddressCC.Split(';', StringSplitOptions.RemoveEmptyEntries))
                email.To.Add(cc);

        // BCC
        if (!string.IsNullOrEmpty(request.AddressBCC))
            foreach (var bcc in request.AddressBCC.Split(';', StringSplitOptions.RemoveEmptyEntries))
                email.To.Add(bcc);

        // Subject
        email.Subject = request.Subject;

        // Body
        email.Body = request.Body;

        // Attachment
        if (str != null)
        {
            var attachment = new Attachment(str, request.FileName);
            email.Attachments.Add(attachment);
        }

        // Host
        var splitHost = request.Host?.Split(':', StringSplitOptions.RemoveEmptyEntries);
        if (splitHost != null || splitHost?.Length == 2)
        {
            string mailHost = splitHost[0];
            int mailPort = int.Parse(splitHost[1]);

            SmtpClient smtp = new()
            {
                Host = mailHost,
                Port = mailPort,
                Credentials = new NetworkCredential(request.AddressFrom, request.Password),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true,
                UseDefaultCredentials = false,
            };

            await smtp.SendMailAsync(email, cancellationToken);
        }
    }

    /// <summary>
    /// Chức năng: lấy thông tin cấu hình
    /// </summary>
    /// <param name="configurationService"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="MessageResultException"></exception>
    private async Task<EmailConfig> GetConfigDataAsync(IConfigurationService configurationService, CancellationToken cancellationToken = default)
    {
        // Get config from DB
        var configurations = await configurationService.GetAllAsync(false, cancellationToken);

        if (configurations.CodeMessage != CodeMessage._0000)
            throw new MessageResultException("Không thể thực hiện lấy config");

        // Process result
        EmailConfig info = new();

        foreach (var configuration in configurations.Data)
        {
            if (configuration.Key == SystemConfig.SystemEmailTestAddressTo)
            {
                info.TestAddressTo = configuration.Value;
                continue;
            }

            if (configuration.Key == SystemConfig.SystemEmailAddressTo)
            {
                info.AddressTo = configuration.Value;
                continue;
            }

            if (configuration.Key == SystemConfig.SystemEmailAddressFrom)
            {
                info.AddressFrom = configuration.Value;
                continue;
            }

            if (configuration.Key == SystemConfig.SystemEmailAddressCC)
            {
                info.AddressCC = configuration.Value;
                continue;
            }

            if (configuration.Key == SystemConfig.SystemEmailAddressBCC)
            {
                info.AddressBCC = configuration.Value;
                continue;
            }

            if (configuration.Key == SystemConfig.SystemEmailPassword)
            {
                info.Password = configuration.Value.MyAesDecrypt(ThirdPartyEncryption.Secret);
                continue;
            }

            if (configuration.Key == SystemConfig.SystemEmailHost)
            {
                info.Host = configuration.Value;
                continue;
            }
        }

        return info;
    }

    #endregion

    #endregion
}