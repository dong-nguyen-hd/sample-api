using EPAY.AIRWAY.KIOSK.API.Domain.Context;
using EPAY.AIRWAY.KIOSK.API.Domain.Models.ReportSection.ToJson;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Payment.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Payment.Response;
using EPAY.AIRWAY.KIOSK.API.Resources.Enums;
using EPAY.AIRWAY.KIOSK.API.Resources.Exceptions;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using PaymentGateway = EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.PaymentGateway;

namespace EPAY.AIRWAY.KIOSK.API.Services;

public sealed class PaymentService(
    ISignalRService signalRService,
    IConfigurationService configurationService,
    IHttpContextAccessor httpContextAccessor,
    IPaymentGatewayService paymentGatewayService,
    IFlightService flightService,
    IMapper mapper,
    CoreContext context) : BaseService, IPaymentService
{
    #region Properties

    private readonly HttpContext? _httpContext = httpContextAccessor?.HttpContext;
    private string? _hostFe;

    #endregion

    #region Process Callback

    public async Task ProcessCallbackAsync(PaymentGateway.Request.BaseRequest<string> request, DateTime utcNow, CancellationToken cancellationToken)
    {
        if (request.Data == null || string.IsNullOrEmpty(request.Signature))
            throw new BadRequestException("[1] Thông tin IPN không hợp lệ");

        var resultPaymentGateway = await paymentGatewayService.DecryptDataCallBackAsync(request, cancellationToken);
        var innerData = resultPaymentGateway.Data;
        if (resultPaymentGateway.CodeMessage != CodeMessage._0000 || innerData == null)
            throw new BadRequestException("[2] Thông tin IPN không hợp lệ");

        // Không xử lí với luồng redirect màn hình
        var paymentTransaction = await context.PaymentTransactions
            .AsNoTracking()
            .Select(x => new Model.PaymentTransaction()
            {
                Id = x.Id,
                PaymentType = x.PaymentType,
                OrderCode = x.OrderCode
            })
            .SingleOrDefaultAsync(x => x.OrderCode == innerData.OrderCode, cancellationToken);

        if (paymentTransaction == null ||
            paymentTransaction.PaymentType == PaymentType.BankAccount ||
            paymentTransaction.PaymentType == PaymentType.LocalCard ||
            paymentTransaction.PaymentType == PaymentType.GlobalCard ||
            paymentTransaction.PaymentType == PaymentType.EpayWallet)
            return;

        // Xử lí payment/check với các trường hợp còn lại
        CheckRequest checkPayload = new()
        {
            OrderCode = innerData.OrderCode,
            IsInternal = true,
            UseNotify = true
        };
        await CheckPaymentAsync(checkPayload, utcNow, cancellationToken);
    }

    #endregion

    #region Check Payment

    public async Task<BaseResult<CheckResponse>> CheckPaymentAsync(CheckRequest request, DateTime utcNow, CancellationToken cancellationToken = default)
    {
        await GetConfigDataAsync(cancellationToken);

        var paymentTransaction = await context.PaymentTransactions
            .Include(x => x.Bill).ThenInclude(x => x.Contact)
            .Include(x => x.Bill).ThenInclude(x => x.Passengers)
            .Include(x => x.Bill).ThenInclude(x => x.Reservations)
            .Include(x => x.Bill).ThenInclude(x => x.FlightDatas)
            .Include(x => x.Bill).ThenInclude(x => x.FareDatas)
            .SingleOrDefaultAsync(x => x.OrderCode == request.OrderCode, cancellationToken);

        // Validate data
        if (paymentTransaction == null)
            return GetBaseResult<CheckResponse>(CodeMessage._9003);
        if (!request.IsInternal && paymentTransaction.BillId != request.BillId)
            return GetBaseResult<CheckResponse>(CodeMessage._9003);

        // Lấy dữ liệu master-data
        var masterData = await flightService.GetMasterDataAsync(false, cancellationToken);

        var bill = paymentTransaction?.Bill;
        if (bill == null || masterData.CodeMessage != CodeMessage._0000)
            return GetBaseResult<CheckResponse>(CodeMessage._9003);

        // Cập nhật thông tin trạng thái thanh toán
        await UpdatePaymentProviderStatusAsync(paymentTransaction, utcNow, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // Cập nhật thông tin trạng thái xuất vé
        await UpdateServiceProviderStatusAsync(paymentTransaction, cancellationToken);

        await UpdateReportAsync(paymentTransaction, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // Public message to SignalR
        if (request.UseNotify && paymentTransaction.ServiceProviderStatus != ServiceStatus.None)
        {
            request.BillId = bill.Id;
            request.OrderCode = paymentTransaction.OrderCode;
            await signalRService.PublicMessageAsync(request, cancellationToken);
        }

        return GetBaseResult(CodeMessage._0000, data: MappingCheckResponse(bill!, paymentTransaction, masterData.Data));
    }

    /// <summary>
    /// Chức năng: cập nhật thông tin report sau khi có kết quả giao dịch.
    /// </summary>
    /// <param name="paymentTransaction"></param>
    /// <param name="cancellationToken"></param>
    private async Task UpdateReportAsync(Model.PaymentTransaction paymentTransaction, CancellationToken cancellationToken = default)
    {
        var bill = paymentTransaction.Bill;

        // Lấy thông tin report
        var report = await context.Reports.SingleOrDefaultAsync(x => x.OrderCode == paymentTransaction.OrderCode, cancellationToken);
        if (report == null)
        {
            Serilog.Log.Error($"Thông tin report không tồn tại (order-code: {paymentTransaction.OrderCode})");
            return;
        }

        // Cập nhật report
        report.UpdatedDatetimeUtc = DateTime.UtcNow;
        report.PartnerPaymentType = paymentTransaction.PartnerPaymentType;
        report.PartnerPaymentStatus = ((int)paymentTransaction.PaymentProviderStatus).ToString();
        report.TransCode = paymentTransaction.TransCode;
        report.DeliveryStatus = paymentTransaction.ServiceProviderStatus == ServiceStatus.Success ? "1" : "0";

        // Bổ sung thông tin vé
        if (report?.OtherInfo?.ListFareData != null && report?.OtherInfo?.ListFareData.Count > 0)
        {
            for (int i = 0; i < report.OtherInfo.ListFareData.Count; i++)
            {
                var fareReport = report.OtherInfo.ListFareData[i];

                var reservation = bill.Reservations.First(x => x.BookingCode.Equals(fareReport.BookingCode, StringComparison.OrdinalIgnoreCase));
                fareReport.ServiceProviderStatus = reservation.TicketIssued;

                if (bill.Tickets != null && bill.Tickets.Count > 0)
                {
                    // Tìm tất cả các vé có cùng booking-code
                    var tickets = bill.Tickets.Where(x => x.BookingCode.Equals(fareReport.BookingCode, StringComparison.OrdinalIgnoreCase)).ToList();

                    // Lấy mã vé người lớn
                    fareReport.TicketNumberAdt = string.Join(", ", tickets
                        .Where(x => x.PassengerType == PassengerType.ADT)
                        .Select(y => y.TicketNumber)
                        .ToList());

                    // Lấy mã vé trẻ em
                    fareReport.TicketNumberChd = string.Join(", ", tickets
                        .Where(x => x.PassengerType == PassengerType.CHD)
                        .Select(y => y.TicketNumber)
                        .ToList());
                }
            }
        }

        context.Reports.Update(report);
    }

    /// <summary>
    /// Chức năng: kiểm tra trạng thái giao dịch từ payment-gateway
    /// </summary>
    /// <param name="paymentTransaction"></param>
    /// <param name="utcNow"></param>
    /// <param name="cancellationToken"></param>
    private async Task UpdatePaymentProviderStatusAsync(Model.PaymentTransaction paymentTransaction, DateTime utcNow, CancellationToken cancellationToken = default)
    {
        try
        {
            if (IsValidPayment(paymentTransaction.PaymentProviderStatus))
            {
                var paymentGatewayResult = await paymentGatewayService.CheckOrderAsync(new()
                {
                    OrderCode = paymentTransaction.OrderCode
                }, utcNow.ConvertUtcToVietnamTz(), cancellationToken);

                paymentTransaction.PaymentProviderStatus = paymentGatewayResult?.Data?.MappingFromPaymentGateway?.PaymentStatus ?? PaymentStatus.Unknown;
                paymentTransaction.TransCode = paymentGatewayResult?.Data?.MappingFromPaymentGateway?.TransCode;
                paymentTransaction.PartnerPaymentType = paymentGatewayResult?.Data?.MappingFromPaymentGateway?.PartnerPaymentType;

                // Gán giá trị thời gian thanh toán
                if (!IsValidPayment(paymentTransaction.PaymentProviderStatus))
                    paymentTransaction.PaidDatetimeUtc = utcNow;

                await UpdatePaymentTransactionAsync(paymentTransaction, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            if (ex is TaskCanceledException or OperationCanceledException)
                paymentTransaction.PaymentProviderStatus = PaymentStatus.Timeout;

            paymentTransaction.PaymentProviderStatus = PaymentStatus.Unknown;
        }
    }

    /// <summary>
    /// Chức năng: gọi xuất vé từ Abtrip
    /// </summary>
    /// <param name="paymentTransaction"></param>
    /// <param name="cancellationToken"></param>
    private async Task UpdateServiceProviderStatusAsync(Model.PaymentTransaction paymentTransaction, CancellationToken cancellationToken = default)
    {
        try
        {
            // Chỉ tiến hành xuất vé khi đã thanh toán thành công
            if (paymentTransaction.PaymentProviderStatus != PaymentStatus.Success)
                return;

            // Logic kiểm tra chỉ gọi issue duy nhất một lần
            if (paymentTransaction.ServiceProviderStatus != ServiceStatus.None)
                return;

            paymentTransaction.ServiceProviderStatus = ServiceStatus.Unknown;
            await context.SaveChangesAsync(cancellationToken);

            // Gọi issue lấy kết quả xuất vé
            var bill = paymentTransaction.Bill;
            var issueResult = await flightService.IssueAsync(new IssueRequest { AbTripOrderId = bill.AbTripOrderId }, cancellationToken);

            // Xử lí kết quả trả về
            paymentTransaction.ServiceProviderStatus = issueResult?.Data?.AllSuccessful == true ? ServiceStatus.Success : ServiceStatus.Fail;
            CreateTicketData(bill, issueResult?.Data);

            await UpdatePaymentTransactionAsync(paymentTransaction, cancellationToken);
        }
        catch (Exception ex)
        {
            if (ex is TaskCanceledException or OperationCanceledException)
                paymentTransaction.ServiceProviderStatus = ServiceStatus.Timeout;

            paymentTransaction.ServiceProviderStatus = ServiceStatus.Unknown;
        }
    }

    /// <summary>
    /// Chức năng: tạo mới thông tin vé cho đơn hàng
    /// </summary>
    /// <param name="bill"></param>
    /// <param name="issueResponse"></param>
    private static void CreateTicketData(Model.Bill? bill, IssueResponse? issueResponse)
    {
        if (bill == null || issueResponse == null || issueResponse.IssueStatus == null)
            return;

        if (bill.Reservations == null || bill.Reservations.Count <= 0)
            return;

        foreach (var reservation in bill.Reservations)
        {
            foreach (var item in issueResponse.IssueStatus)
            {
                if (item.Key.Equals(reservation.BookingCode, StringComparison.OrdinalIgnoreCase))
                {
                    DateTime utcNow = DateTime.UtcNow;
                    reservation.UpdatedDatetimeUtc = utcNow;
                    reservation.TicketIssued = item.Value.TicketIssued;

                    // Lưu thông tin ticket
                    if (item.Value?.Tickets != null && item.Value.Tickets.Count > 0)
                    {
                        bill.Tickets = new();
                        foreach (var ticket in item.Value.Tickets)
                        {
                            bill.Tickets.Add(new()
                            {
                                BookingCode = item.Key,
                                TicketNumber = ticket.TicketNumber,
                                IssueDatetimeUtc = ticket.IssueDatetimeUtc,
                                PassengerType = ticket.PassengerType,
                                TotalPrice = ticket.TotalPrice,
                                Active = true,
                                UpdatedDatetimeUtc = utcNow,
                                CreatedDatetimeUtc = utcNow
                            });
                        }
                    }
                }
            }

            if (issueResponse.AllSuccessful)
            {
                reservation.TicketIssued = true;
                reservation.UpdatedDatetimeUtc = DateTime.UtcNow;
            }
        }
    }

    /// <summary>
    /// Chức năng: cập nhật thông tin trạng thái giao dịch vào DB
    /// </summary>
    /// <param name="paymentTransaction"></param>
    /// <param name="cancellationToken"></param>
    private async Task UpdatePaymentTransactionAsync(Model.PaymentTransaction paymentTransaction, CancellationToken cancellationToken = default)
    {
        // Bổ sung tracking trans
        DateTime tempUtc = DateTime.UtcNow;
        paymentTransaction.UpdatedDatetimeUtc = tempUtc;
        Model.TransactionTracking tracking = new()
        {
            PaymentTransactionId = paymentTransaction.Id,
            PaymentProviderStatus = paymentTransaction.PaymentProviderStatus,
            ServiceProviderStatus = paymentTransaction.ServiceProviderStatus,
            Active = true,
            CreatedDatetimeUtc = tempUtc,
            UpdatedDatetimeUtc = tempUtc
        };

        await context.AddAsync(tracking, cancellationToken);
        context.Update(paymentTransaction);
    }

    /// <summary>
    /// Chức năng: xác định trạng thái kết thúc của thanh toán <br/>
    /// Bao gồm: <br/>
    /// true - các trạng thái chưa thanh toán <br/>
    /// false - thất bại, thành công, đã thanh toán <br/>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    private static bool IsValidPayment(PaymentStatus source)
    {
        switch (source)
        {
            case PaymentStatus.None:
            case PaymentStatus.Timeout:
            case PaymentStatus.Init:
            case PaymentStatus.Pending:
            case PaymentStatus.Unknown:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Chức năng: xác định trạng thái cuối của dịch vụ <br/>
    /// Bao gồm: <br/>
    /// true - các trạng thái chưa xuất vé <br/>
    /// false - thất bại, thành công, đã xuất vé <br/>
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    private static bool IsValidService(ServiceStatus source)
    {
        switch (source)
        {
            case ServiceStatus.None:
            case ServiceStatus.Timeout:
            case ServiceStatus.Unknown:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Chức năng: tạo dữ liệu trả về cho service
    /// </summary>
    /// <param name="bill"></param>
    /// <param name="paymentTransaction"></param>
    /// <param name="masterData"></param>
    /// <returns></returns>
    private CheckResponse MappingCheckResponse(Model.Bill bill, Model.PaymentTransaction paymentTransaction, MasterDataResponse? masterData)
    {
        CheckResponse result = new()
        {
            PaymentType = ConvertPaymentType(paymentTransaction),
            PlatformType = paymentTransaction.PlatformType,
            OrderCode = paymentTransaction.OrderCode,
            BillId = paymentTransaction.BillId,
            AbTripOrderId = bill.AbTripOrderId,
            TotalAmount = paymentTransaction.TotalAmount,
            TicketIssueStatus = ConvertTicketIssueStatus(paymentTransaction),
            PaidDatetimeUtc = paymentTransaction.PaidDatetimeUtc,
            ExpiredDatetimeUtc = paymentTransaction.ExpiredDatetimeUtc
        };

        var contact = bill?.Contact;
        result.Service = new()
        {
            BookingDatetimeUtc = bill?.CreatedDatetimeUtc,
            Contact = contact != null
                ? new()
                {
                    FirstName = contact.FirstName,
                    LastName = contact.LastName,
                    Email = contact.Email,
                    Gender = contact.Gender,
                    Phone = contact.Phone
                }
                : new(),
            TicketType = flightService.ConvertTicketType(bill!.FlightType),
            TotalTicket = 0,
        };

        // Tính total-ticket
        var firstFare = bill?.FareDatas?.FirstOrDefault();
        if (bill?.FlightType is FlightType.InternationalOneWay or FlightType.InternationalRoundTrip or FlightType.DomesticOneWay)
            result.Service.TotalTicket = (firstFare?.Adt + firstFare?.Chd) ?? 0;
        else if (bill?.FlightType is FlightType.DomesticRoundTrip)
            result.Service.TotalTicket = (firstFare?.Adt + firstFare?.Chd) * 2 ?? 0;
        else
            result.Service.TotalTicket = 0;

        // Mapping start/end point
        if (bill?.FlightDatas != null && bill.FlightDatas.Count > 0)
        {
            if (bill.FlightDatas.Count == 1)
            {
                var firstFlight = bill.FlightDatas.First();
                result.Service.PointOne = new()
                {
                    BookingCode = firstFlight.BookingCode,
                    TicketIssued = bill?.Reservations?.FirstOrDefault(x => x.Active && x.BookingCode.Equals(firstFlight.BookingCode, StringComparison.OrdinalIgnoreCase)).TicketIssued ?? false,
                    Airline = masterData?.Airlines?.Find(x => x.Code!.Equals(firstFlight.Airline)),
                    StartPoint = masterData?.Airports?.Find(x => x.Code!.Equals(firstFlight.StartPoint)),
                    StartDate = firstFlight.StartDate,
                    EndPoint = masterData?.Airports?.Find(x => x.Code!.Equals(firstFlight.EndPoint)),
                    EndDate = firstFlight.EndDate
                };
            }
            else if (bill.FlightDatas.Count == 2)
            {
                var firstFlight = bill.FlightDatas.First(x => x.Departure);
                var lastFlight = bill.FlightDatas.First(x => !x.Departure);

                result.Service.PointOne = new()
                {
                    BookingCode = firstFlight.BookingCode,
                    TicketIssued = bill?.Reservations?.FirstOrDefault(x => x.Active && x.BookingCode.Equals(firstFlight.BookingCode, StringComparison.OrdinalIgnoreCase)).TicketIssued ?? false,
                    Airline = masterData?.Airlines?.Find(x => x.Code!.Equals(firstFlight.Airline)),
                    StartPoint = masterData?.Airports?.Find(x => x.Code!.Equals(firstFlight.StartPoint)),
                    StartDate = firstFlight.StartDate,
                    EndPoint = masterData?.Airports?.Find(x => x.Code!.Equals(firstFlight.EndPoint)),
                    EndDate = firstFlight.EndDate
                };

                result.Service.PointTwo = new()
                {
                    BookingCode = lastFlight.BookingCode,
                    TicketIssued = bill?.Reservations?.FirstOrDefault(x => x.Active && x.BookingCode.Equals(lastFlight.BookingCode, StringComparison.OrdinalIgnoreCase)).TicketIssued ?? false,
                    Airline = masterData?.Airlines?.Find(x => x.Code!.Equals(lastFlight.Airline)),
                    StartPoint = masterData?.Airports?.Find(x => x.Code!.Equals(lastFlight.StartPoint)),
                    StartDate = lastFlight.StartDate,
                    EndPoint = masterData?.Airports?.Find(x => x.Code!.Equals(lastFlight.EndPoint)),
                    EndDate = lastFlight.EndDate
                };
            }
        }

        return result;
    }

    /// <summary>
    /// Chức năng: lấy ra phương thức thanh toán thực tế của người dùng
    /// </summary>
    /// <param name="paymentTransaction"></param>
    /// <returns></returns>
    private MyEnum.PaymentType ConvertPaymentType(Model.PaymentTransaction paymentTransaction)
    {
        if (string.IsNullOrEmpty(paymentTransaction.PartnerPaymentType))
            return paymentTransaction.PaymentType;

        return paymentGatewayService.GetPaymentType(paymentTransaction.PartnerPaymentType);
    }

    /// <summary>
    /// Chức năng: chuyển đổi kiểu trạng thái thành toán/dịch vụ => trạng thái hiển thị kết quả
    /// </summary>
    /// <param name="paymentTransaction"></param>
    /// <returns></returns>
    private static TicketIssueStatus ConvertTicketIssueStatus(Model.PaymentTransaction paymentTransaction)
    {
        if (paymentTransaction.PaymentProviderStatus == PaymentStatus.Success)
            return TicketIssueStatus.Success;

        return TicketIssueStatus.Fail;
    }

    #endregion

    #region Generate Payment

    public async Task<BaseResult<GenerateResponse>> GeneratePaymentAsync(GenerateRequest request, DateTime utcNow, CancellationToken cancellationToken = default)
    {
        await GetConfigDataAsync(cancellationToken);

        // Kiểm tra BillId hợp lệ
        var bill = await context.Bills
            .AsNoTracking()
            .Include(x => x.FlightDatas)
            .Include(x => x.FareDatas)
            .Include(x => x.Tickets)
            .Include(x => x.PaymentTransactions.Where(y => y.PaymentProviderStatus == PaymentStatus.Success))
            .Include(x => x.Passengers)
            .ThenInclude(x => x.AdditionalServices)
            .SingleOrDefaultAsync(x => x.Id == request.BillId, cancellationToken);
        if (bill == null)
            return GetBaseResult<GenerateResponse>(CodeMessage._9004);
        if (bill?.PaymentTransactions?.Count > 0)
            return GetBaseResult<GenerateResponse>(CodeMessage._9001);

        // Lấy thông tin về POS nếu hình thức thanh toán là POS
        Model.ReportSection.Device? device = null;
        if (request.PaymentType == PaymentType.Pos)
        {
            string? code = GetDeviceId();
            if (string.IsNullOrEmpty(code))
                return GetBaseResult<GenerateResponse>(CodeMessage._9002);

            device = await context.Devices
                .Include(x => x.Location)
                .Include(x => x.ServicePartner)
                .ThenInclude(x => x.SaleChannel)
                .SingleOrDefaultAsync(x => x.Id == new Guid(code), cancellationToken);

            if (device == null)
                return GetBaseResult<GenerateResponse>(CodeMessage._9002);
        }

        var paymentTransaction = CreatePaymentTransaction(request, device, bill!, utcNow);

        try
        {
            paymentTransaction = await GenerateOrderAsync(paymentTransaction, bill!, utcNow, cancellationToken);
        }
        catch (Exception ex)
        {
            Serilog.Log.Error($"Lỗi khởi tạo thanh toán: {ex.Message} >>> {ex.StackTrace}", ex);

            if (ex is TaskCanceledException or OperationCanceledException)
                paymentTransaction.PaymentProviderStatus = PaymentStatus.Timeout;

            paymentTransaction.PaymentProviderStatus = PaymentStatus.Unknown;
        }

        try
        {
            // Lưu thông tin phục vụ bóc tách dữ liệu
            await SaveReportAsync(paymentTransaction, bill, device, cancellationToken);

            // Lưu thông tin payment tracking
            paymentTransaction.TransactionTrackings = new()
            {
                new()
                {
                    TraceId = paymentTransaction.TraceId,
                    ServiceProviderStatus = paymentTransaction.ServiceProviderStatus,
                    PaymentProviderStatus = paymentTransaction.PaymentProviderStatus,
                    Active = true,
                    CreatedDatetimeUtc = utcNow,
                    UpdatedDatetimeUtc = utcNow,
                }
            };

            await context.AddAsync(paymentTransaction, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // Cơ chế log file khi lưu log - db thất bại
            Serilog.Log.Error($"Lỗi khi lưu trans: {ex.Message} >>>> {paymentTransaction.MySerialize()}", ex);

            throw;
        }

        // Mapping result
        var result = mapper.Map<GenerateResponse>(paymentTransaction);
        result.RequestDatetimeUtc = utcNow;
        result.SecondsExpiration = result.ExpiredDatetimeUtc != null ? (int)(result.ExpiredDatetimeUtc.Value - DateTime.UtcNow).TotalSeconds : 0;

        // Process result
        if (paymentTransaction.PaymentProviderStatus == PaymentStatus.Success ||
            paymentTransaction.PaymentProviderStatus == PaymentStatus.None ||
            paymentTransaction.PaymentProviderStatus == PaymentStatus.Init)
            return GetBaseResult(CodeMessage._0000, data: result);

        return GetBaseResult(CodeMessage._9002, data: result);
    }

    /// <summary>
    /// Chức năng: khởi tạo giao dịch sang cổng thanh toán
    /// </summary>
    /// <param name="paymentTransaction"></param>
    /// <param name="bill"></param>
    /// <param name="utcNow"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="MessageResultException"></exception>
    private async Task<Model.PaymentTransaction> GenerateOrderAsync(Model.PaymentTransaction paymentTransaction, Model.Bill bill, DateTime utcNow, CancellationToken cancellationToken = default)
    {
        var paymentGatewayConfig = await paymentGatewayService.GetConfigDataAsync(cancellationToken);
        if (paymentGatewayConfig == null)
            throw new MessageResultException("Có lỗi xảy ra khi lấy thông tin cấu hình");

        string redirectLink = $"{_hostFe}{paymentTransaction.ReturnUrl}&orderCode={paymentTransaction.OrderCode}";
        string? orderDescription = paymentGatewayConfig?.Config?.OrderDescription?.Replace("[0]", bill.AbTripOrderId);
        var timeLimit = paymentGatewayService.GetTimeLimit(paymentTransaction.PaymentType, paymentGatewayConfig);

        // Gán dữ liệu cho payment-transaction
        paymentTransaction.ExpiredDatetimeUtc = utcNow.AddMinutes(timeLimit);
        paymentTransaction.ReturnUrl = redirectLink;

        // Request to payment-gateway
        var paymentGatewayResult = await paymentGatewayService.CreateOrderAsync(new()
        {
            ChannelCode = paymentGatewayService.GetChannelCode(paymentTransaction.PlatformType, paymentTransaction.PaymentType),
            KioskId = paymentTransaction.DeviceCode,
            PosSerial = paymentTransaction.PosSerial,
            PosRefId = paymentTransaction.PosRefId,
            PosMerchantId = paymentTransaction.PosMerchantId,
            PosClientId = paymentTransaction.PosClientId,
            PosMerchantOutletId = paymentTransaction.PosMerchantOutletId,
            PosTerminalId = paymentTransaction.PosTerminalId,
            PaymentMethod = paymentGatewayService.GetPaymentMethod(paymentTransaction.PaymentType),
            MerchantCode = paymentGatewayConfig.Config?.MerchantCode,
            MerchantPassword = paymentGatewayConfig.Config?.Password,
            OrderCode = paymentTransaction.OrderCode,
            BillId = paymentTransaction.BillId,
            PaymentType = 1,
            TotalAmount = paymentTransaction.TotalAmount,
            OrderAmount = paymentTransaction.TotalAmount,
            OrderDescription = orderDescription,
            CustomerFullName = string.Empty,
            CustomerIdNumber = paymentTransaction.CustomerIdNumber,
            CustomerEmail = paymentTransaction.CustomerEmail,
            CustomerMobile = paymentTransaction.CustomerMobile,
            CustomerAddress = paymentTransaction.CustomerAddress,
            ReturnUrl = redirectLink,
            CancelUrl = redirectLink,
            AgainUrl = redirectLink,
            TypeCardAccount = paymentGatewayService.GetTypeCardAcount(paymentTransaction.PaymentType),
            TimeLimit = timeLimit,
            WalletFunctionType = paymentGatewayService.GetWalletFunctionType(paymentTransaction.PlatformType, paymentTransaction.PaymentType),
            TotalGoods = 1,
            DetailGoods = new List<PaymentGateway.Request.DetailInfo>()
            {
                new()
                {
                    GoodsCode = paymentTransaction.BillId,
                    GoodsName = orderDescription,
                    GoodsUrl = redirectLink,
                    GoodsQuantity = 1,
                    GoodsPrice = paymentTransaction.TotalAmount,
                }
            },
            AgencyCode = paymentGatewayConfig.Config?.AgencyCode,
            AgencyName = paymentGatewayConfig.Config?.AgencyName,
            Provider = paymentGatewayConfig.Config?.Provider,
        }, utcNow.ConvertUtcToVietnamTz(), cancellationToken);

        if (paymentGatewayResult.CodeMessage == CodeMessage._0000)
        {
            paymentTransaction.PaymentProviderStatus = PaymentStatus.Init;
            paymentTransaction.Qr = paymentGatewayResult?.Data?.QrCode;
            paymentTransaction.PaymentUrl = paymentGatewayResult?.Data?.PaymentUrl;
            paymentTransaction.PaymentDeeplink = paymentGatewayResult?.Data?.DeepLink;

            // Bổ sung tracking
            DateTime tempUtcNow = DateTime.UtcNow;
            paymentTransaction.TransactionTrackings?.Add(new()
            {
                ServiceProviderStatus = paymentTransaction.ServiceProviderStatus,
                PaymentProviderStatus = paymentTransaction.PaymentProviderStatus,
                Active = true,
                CreatedDatetimeUtc = tempUtcNow,
                UpdatedDatetimeUtc = tempUtcNow,
            });
        }
        else
            paymentTransaction.PaymentProviderStatus = PaymentStatus.Fail;

        return paymentTransaction;
    }

    /// <summary>
    /// Chức năng: tạo thông tin payment-transaction
    /// </summary>
    /// <param name="request"></param>
    /// <param name="device"></param>
    /// <param name="bill"></param>
    /// <param name="utcNow"></param>
    /// <returns></returns>
    private Model.PaymentTransaction CreatePaymentTransaction(GenerateRequest request, Model.ReportSection.Device? device, Model.Bill bill, DateTime utcNow)
    {
        Model.PaymentTransaction paymentTransaction = new()
        {
            IsPaylater = request.IsPaylater,
            TraceId = _httpContext != null ? _httpContext.TraceIdentifier : RelateText.GenId(),
            PaymentType = request.PaymentType,
            OrderCode = RelateText.GenId(),
            BillId = request.BillId!,
            CustomerFullName = request?.Customer?.FullName,
            CustomerEmail = request?.Customer?.Email,
            CustomerMobile = request?.Customer?.Mobile,
            CustomerAddress = request?.Customer?.Address,
            CustomerIdNumber = request?.Customer?.IdNumber,
            ServiceProviderStatus = ServiceStatus.None,
            PaymentProviderStatus = PaymentStatus.None,
            ReturnUrl = request?.ReturnUrl,
            PosSerial = device?.PosSerial,
            PosRefId = device?.PosRefId,
            PosMerchantId = device?.PosMerchantId,
            PosClientId = device?.PosClientId,
            PosMerchantOutletId = device?.PosMerchantOutletId,
            PosTerminalId = device?.PosTerminalId,
            DeviceCode = device?.Id.ToString(),
            TotalAmount = bill!.TotalPrice,
            PlatformType = request!.PlatformType,
            Active = true,
            CreatedDatetimeUtc = utcNow,
            UpdatedDatetimeUtc = utcNow,
        };

        return paymentTransaction;
    }

    /// <summary>
    /// Chức năng: tạo thông tin bóc tách cho báo cáo
    /// </summary>
    /// <param name="paymentTransaction"></param>
    /// <param name="bill"></param>
    /// <param name="device"></param>
    /// <param name="cancellationToken"></param>
    /// <exception cref="MessageResultException"></exception>
    private async Task SaveReportAsync(Model.PaymentTransaction paymentTransaction, Model.Bill bill, Model.ReportSection.Device? device, CancellationToken cancellationToken = default)
    {
        // Lấy thông tin sale-channel
        var saleChannel = await context.SalesChannel
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SystemPlatformType == paymentTransaction.PlatformType, cancellationToken);

        if (saleChannel == null)
            throw new MessageResultException("Cấu hình mã kênh bán không tồn tại");

        // Khởi tạo thông tin report
        Model.ReportSection.Report report = new()
        {
            Id = Guid.NewGuid(),
            CreatedDatetimeUtc = paymentTransaction.CreatedDatetimeUtc,
            UpdatedDatetimeUtc = paymentTransaction.CreatedDatetimeUtc,
            OrderCode = paymentTransaction.OrderCode,
            BillCode = paymentTransaction.BillId,
            PaymentType = ((int)paymentTransaction.PaymentType).ToString(),
            PartnerPaymentType = paymentTransaction.PartnerPaymentType,
            PartnerPaymentStatus = ((int)paymentTransaction.PaymentProviderStatus).ToString(),
            TransCode = paymentTransaction.TransCode,
            DeliveryStatus = "0", // Giao dịch vừa khởi tạo, nên mặc định tính là chưa xuất vé với giá trị "0"
            PaymentAmount = paymentTransaction.TotalAmount,
            SaleChannelCode = saleChannel.Code,
            Active = true
        };

        // Gán các thông tin liên quan tới device
        if (device != null)
        {
            report.ServicePartnerCode = device.ServicePartner.Code;
            report.LocationCode = device.Location.Code;
            report.DeviceSerial = device.Serial;
        }

        // Gán các thông tin liên quan tới chuyến bay
        // Lưu ý: các thông tin về ticket ở bước này chưa có
        var flightStart = bill.FlightDatas!.First(x => x.Departure);
        var fareStart = bill.FareDatas!.First(x => x.AbTripFareDataId!.Equals(flightStart.AbTripFareDataId, StringComparison.OrdinalIgnoreCase));

        var flightEnd = bill.FlightDatas!.FirstOrDefault(x => !x.Departure);

        var additionalServices = bill.Passengers?.SelectMany(x => x.AdditionalServices).ToList();

        report.OtherInfo = new OtherInfo()
        {
            OrderId = bill.AbTripOrderId,
            StartPoint = flightStart.StartPoint,
            EndPoint = flightStart.EndPoint,
            TicketType = ((int)flightService.ConvertTicketType(bill.FlightType)).ToString(),
            JourneyType = ((int)flightService.ConvertJourneyType(bill.FlightType)).ToString(),
        };

        // Gán thông tin hành lí bổ sung
        List<ServiceData>? listBaggage;
        List<ServiceData>? listAncillary;
        if (bill.FlightType == FlightType.InternationalRoundTrip)
        {
            listBaggage = additionalServices?
                .Where(x => x.Type == AdditionalServiceType.Baggage)
                .Select(x => new ServiceData()
                {
                    Name = x.Name,
                    Price = x.Price
                }).ToList();

            listAncillary = additionalServices?
                .Where(x => x.Type == AdditionalServiceType.Service)
                .Select(x => new ServiceData()
                {
                    Name = x.Name,
                    Price = x.Price
                }).ToList();
        }
        else
        {
            listBaggage = additionalServices?
                .Where(x => x.Type == AdditionalServiceType.Baggage && (x?.StartPoint?.Equals(flightStart.StartPoint, StringComparison.OrdinalIgnoreCase) ?? false))
                .Select(x => new ServiceData()
                {
                    Name = x.Name,
                    Price = x.Price
                }).ToList();

            listAncillary = additionalServices?
                .Where(x => x.Type == AdditionalServiceType.Service && (x?.StartPoint?.Equals(flightStart.StartPoint, StringComparison.OrdinalIgnoreCase) ?? false))
                .Select(x => new ServiceData()
                {
                    Name = x.Name,
                    Price = x.Price
                }).ToList();
        }

        // Gán thông tin chuyến bay khởi hành
        // Trương hợp thông tin chuyến bay là quốc tế - khứ hồi, thì gộp chung vào một phần tử trong ListFareData
        report.OtherInfo.ListFareData = new()
        {
            new()
            {
                IsDeparture = bill.FlightType == FlightType.InternationalRoundTrip ? null : true,
                BookingCode = flightStart.BookingCode,
                TicketQuantityAdt = fareStart.Adt.ToString(),
                TicketQuantityChd = fareStart.Chd.ToString(),
                ServiceProviderStatus = false,
                TotalPrice = fareStart.TotalPrice + listBaggage?.Sum(x => x.Price) + listAncillary?.Sum(x => x.Price),
                ListBaggage = listBaggage,
                ListAncillary = listAncillary,
            }
        };

        // Gán thông tin chuyến bay kết thúc
        if (bill.FlightType != FlightType.InternationalRoundTrip && flightEnd != null)
        {
            var fareEnd = bill.FareDatas!.First(x => x.AbTripFareDataId!.Equals(flightEnd.AbTripFareDataId, StringComparison.OrdinalIgnoreCase));

            // Gán thông tin hành lí bổ sung
            List<ServiceData>? listBaggageEnd = additionalServices?
                .Where(x => x.Type == AdditionalServiceType.Baggage && (x?.StartPoint?.Equals(flightEnd.StartPoint, StringComparison.OrdinalIgnoreCase) ?? false))
                .Select(x => new ServiceData()
                {
                    Name = x.Name,
                    Price = x.Price
                }).ToList();

            List<ServiceData>? listAncillaryEnd = additionalServices?
                .Where(x => x.Type == AdditionalServiceType.Service && (x?.StartPoint?.Equals(flightEnd.StartPoint, StringComparison.OrdinalIgnoreCase) ?? false))
                .Select(x => new ServiceData()
                {
                    Name = x.Name,
                    Price = x.Price
                }).ToList();

            report.OtherInfo.ListFareData.Add(new()
            {
                IsDeparture = bill.FlightType == FlightType.InternationalRoundTrip ? null : false,
                BookingCode = flightEnd.BookingCode,
                TicketQuantityAdt = fareEnd.Adt.ToString(),
                TicketQuantityChd = fareEnd.Chd.ToString(),
                ServiceProviderStatus = false,
                TotalPrice = fareEnd.TotalPrice + listBaggageEnd?.Sum(x => x.Price) + listAncillaryEnd?.Sum(x => x.Price),
                ListBaggage = listBaggageEnd,
                ListAncillary = listAncillaryEnd,
            });
        }

        await context.AddAsync(report, cancellationToken);
    }

    /// <summary>
    /// Chức năng: lấy ra thông tin device-id
    /// </summary>
    /// <returns></returns>
    private string? GetDeviceId()
    {
        if (_httpContext?.Request?.Headers == null)
            return string.Empty;

        foreach (var key in _httpContext.Request.Headers.Keys)
            if (!string.IsNullOrEmpty(key) &&
                key.Equals(SystemConstant.DeviceHeaderKey, StringComparison.OrdinalIgnoreCase) &&
                _httpContext.Request.Headers.TryGetValue(key, out var value))
                return value;

        return string.Empty;
    }

    #endregion

    #region Private work

    /// <summary>
    /// Chức năng: lấy dữ liệu cấu hình
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <exception cref="MessageResultException"></exception>
    private async Task GetConfigDataAsync(CancellationToken cancellationToken = default)
    {
        // Get config from DB
        var configurations = await configurationService.GetAllAsync(false, cancellationToken);

        if (configurations.CodeMessage != CodeMessage._0000)
            throw new MessageResultException("Không thể thực hiện lấy config");

        foreach (var configuration in configurations.Data)
        {
            // Config
            if (configuration.Key == SystemConfig.SystemFeHost)
            {
                this._hostFe = configuration.Value!;
                break;
            }
        }
    }

    #endregion
}