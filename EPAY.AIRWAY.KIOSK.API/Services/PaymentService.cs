using EPAY.AIRWAY.KIOSK.API.Domain.Context;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Payment.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Payment.Response;
using EPAY.AIRWAY.KIOSK.API.Resources.Enums;
using EPAY.AIRWAY.KIOSK.API.Resources.Exceptions;
using Hangfire;
using Microsoft.EntityFrameworkCore;
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

    public async Task ProcessCallbackAsync(PaymentGateway.Request.BaseRequest<string> request, CancellationToken cancellationToken)
    {
        var resultPaymentGateway = await paymentGatewayService.DecryptDataCallBackAsync(request, cancellationToken);
        var innerData = resultPaymentGateway.Data;
        if (resultPaymentGateway.CodeMessage != CodeMessage._0000 || innerData == null)
            return;

        var paymentTransaction = await context.PaymentTransactions
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.OrderCode == innerData.OrderCode, cancellationToken);

        if (paymentTransaction != null)
        {
            CheckRequest checkPayload = new()
            {
                OrderCode = paymentTransaction.OrderCode,
                BillId = paymentTransaction.BillId
            };
            await CheckPaymentAsync(checkPayload, DateTime.UtcNow.ConvertUtcToVietnamTz(), cancellationToken);

            // Public message to SignalR
            await signalRService.PublicMessage(checkPayload);
        }
    }

    #endregion

    #region Check Payment

    public async Task<BaseResult<CheckResponse>> CheckPaymentAsync(CheckRequest request, DateTime utcNow, CancellationToken cancellationToken = default)
    {
        await GetConfigDataAsync(cancellationToken);

        var paymentTransaction = await context.PaymentTransactions
            .SingleOrDefaultAsync(x => x.OrderCode == request.OrderCode && x.BillId == request.BillId, cancellationToken);

        // Validate data
        if (paymentTransaction == null)
            return GetBaseResult<CheckResponse>(CodeMessage._9004);

        // Lấy dữ liệu master-data
        var masterData = await flightService.GetMasterDataAsync(false, cancellationToken);

        var bill = await context.Bills
            .AsSplitQuery()
            .Include(x => x.Contact)
            .Include(x => x.Passengers)
            .Include(x => x.Reservations)
            .Include(x => x.FlightDatas)
            .Include(x => x.FareDatas)
            .SingleOrDefaultAsync(x => x.Id == request.BillId, cancellationToken);
        
        if (bill == null)
            return GetBaseResult<CheckResponse>(CodeMessage._9004);

        // Gọi lại hàm kiểm tra giao dịch nếu trạng thái lúc này vẫn chưa kết thúc (successs, fail,...)
        if (IsValidService(paymentTransaction.ServiceProviderStatus))
        {
            try
            {
                if (IsValidPayment(paymentTransaction.PaymentProviderStatus))
                {
                    var paymentGatewayResult = await paymentGatewayService.CheckOrderAsync(new()
                    {
                        OrderCode = request.OrderCode
                    }, utcNow.ConvertUtcToVietnamTz(), cancellationToken);

                    paymentTransaction.PaymentProviderStatus = paymentGatewayResult?.Data?.PaymentStatus ?? PaymentStatus.Unknown;

                    // Lấy ra giá trị phương thức thanh toán thực tế
                    var actualPaymentMethod = paymentGatewayResult?.Data?.TransactionInfos?.FirstOrDefault();
                    if (actualPaymentMethod != null)
                    {
                        var actualType = actualPaymentMethod.PaymentMethod;
                        paymentTransaction.PartnerPaymentType = actualType != 0 ? actualType?.ToString("D2") : actualType.ToString();
                        paymentTransaction.TransCode = actualPaymentMethod.TransCode;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is TaskCanceledException or OperationCanceledException)
                    paymentTransaction.PaymentProviderStatus = PaymentStatus.Timeout;

                paymentTransaction.PaymentProviderStatus = PaymentStatus.Unknown;
            }

            try
            {
                if (paymentTransaction.PaymentProviderStatus == PaymentStatus.Success)
                {
                    var issueResult = await flightService.IssueAsync(new IssueRequest { AbTripOrderId = bill.AbTripOrderId }, cancellationToken);
                    if (issueResult.CodeMessage != CodeMessage._0000)
                    {
                        paymentTransaction.ServiceProviderStatus = ServiceStatus.Fail;
                    }
                    else
                    {
                        bool allSuccess = false;
                        bool isValid = true;
                        foreach (var item in issueResult!.Data!.IssueStatus)
                        {
                            allSuccess = item.Value;
                            var reservation = bill?.Reservations?.FirstOrDefault(x => x.BookingCode?.Equals(item.Key, StringComparison.OrdinalIgnoreCase) ?? false);
                            if (reservation == null) // Nếu booking-code trong issue không tồn tại trong DB => lỗi không xác định
                            {
                                isValid = false;
                                break;
                            }

                            reservation.TicketIssued = item.Value;
                        }

                        if (isValid)
                            paymentTransaction.ServiceProviderStatus = allSuccess ? ServiceStatus.Success : ServiceStatus.HalfSuccess;
                        else
                            paymentTransaction.ServiceProviderStatus = ServiceStatus.Unknown;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is TaskCanceledException or OperationCanceledException)
                    paymentTransaction.ServiceProviderStatus = ServiceStatus.Timeout;

                paymentTransaction.ServiceProviderStatus = ServiceStatus.Unknown;
            }

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
            context.Update(bill!);
            context.Update(paymentTransaction);
            await context.SaveChangesAsync(cancellationToken);
        }

        return GetBaseResult(CodeMessage._0000, data: MappingCheckResponse(bill!, paymentTransaction, masterData.Data));
    }

    /// <summary>
    /// Chức năng: xác định trạng thái kết thúc của thanh toán
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    private static bool IsValidPayment(PaymentStatus source)
    {
        if (source == PaymentStatus.None)
            return true;
        if (source == PaymentStatus.Timeout)
            return true;
        if (source == PaymentStatus.Init)
            return true;
        if (source == PaymentStatus.Pending)
            return true;
        if (source == PaymentStatus.Unknown)
            return true;

        return false;
    }

    private static bool IsValidService(ServiceStatus source)
    {
        if (source == ServiceStatus.None)
            return true;
        if (source == ServiceStatus.Timeout)
            return true;
        if (source == ServiceStatus.Unknown)
            return true;

        return false;
    }

    private static CheckResponse MappingCheckResponse(Model.Bill bill, Model.PaymentTransaction paymentTransaction, MasterDataResponse? masterData)
    {
        CheckResponse result = new()
        {
            PaymentType = paymentTransaction.PaymentType,
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
        var firstFare = bill?.FareDatas?.FirstOrDefault();

        result.Service = new()
        {
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
            TicketType = bill!.TicketType,
            TotalTicket = firstFare?.Adt + firstFare?.Chd ?? 0,
        };

        // Mapping start/end point
        if (bill?.FlightDatas != null && bill.FlightDatas.Count > 0)
        {
            if (bill.FlightDatas.Count == 1)
            {
                var firstFlight = bill.FlightDatas.First();
                result.Service.PointOne = new()
                {
                    BookingCode = firstFlight.BookingCode,
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
                    TicketIssued = paymentTransaction.ServiceProviderStatus == ServiceStatus.Success,
                    Airline = masterData?.Airlines?.Find(x => x.Code!.Equals(firstFlight.Airline)),
                    StartPoint = masterData?.Airports?.Find(x => x.Code!.Equals(firstFlight.StartPoint)),
                    StartDate = firstFlight.StartDate,
                    EndPoint = masterData?.Airports?.Find(x => x.Code!.Equals(firstFlight.EndPoint)),
                    EndDate = firstFlight.EndDate
                };

                result.Service.PointTwo = new()
                {
                    BookingCode = lastFlight.BookingCode,
                    TicketIssued = paymentTransaction.ServiceProviderStatus == ServiceStatus.Success,
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

    private static TicketIssueStatus ConvertTicketIssueStatus(Model.PaymentTransaction paymentTransaction)
    {
        if (paymentTransaction.PaymentProviderStatus == PaymentStatus.Success)
        {
            if (paymentTransaction.ServiceProviderStatus == ServiceStatus.Success)
                return TicketIssueStatus.Success;
            if (paymentTransaction.ServiceProviderStatus == ServiceStatus.HalfSuccess)
                return TicketIssueStatus.HalfSuccess;

            return TicketIssueStatus.Fail;
        }

        return TicketIssueStatus.Fail;
    }

    #endregion

    #region Generate Payment

    public async Task<BaseResult<GenerateResponse>> GeneratePaymentAsync(GenerateRequest request, DateTime utcNow, CancellationToken cancellationToken = default)
    {
        await GetConfigDataAsync(cancellationToken);

        // Kiểm tra đơn hàng đã được thanh toán
        var hasValue = await context.PaymentTransactions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.BillId == request.BillId && x.PaymentProviderStatus == PaymentStatus.Success, cancellationToken);
        if (hasValue != null)
            return GetBaseResult<GenerateResponse>(CodeMessage._9003);

        // Lấy thông tin về POS nếu hình thức thanh toán là POS
        Model.Device? device = null;
        if (request.PaymentType == PaymentType.Pos)
        {
            string? code = GetDeviceId();
            if (string.IsNullOrEmpty(code))
                return GetBaseResult<GenerateResponse>(CodeMessage._9001);

            device = await context.Devices.SingleOrDefaultAsync(x => x.Code == code, cancellationToken);

            if (device == null)
                return GetBaseResult<GenerateResponse>(CodeMessage._9001);
        }

        var paymentTransaction = CreatePaymentTransaction(request, device, utcNow);

        try
        {
            paymentTransaction = await GenerateOrderAsync(paymentTransaction, utcNow, cancellationToken);
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

        // Process result
        if (paymentTransaction.PaymentProviderStatus == PaymentStatus.Success || paymentTransaction.PaymentProviderStatus == PaymentStatus.Init)
            return GetBaseResult(CodeMessage._0000, data: result);

        return GetBaseResult(CodeMessage._9001, data: result);
    }

    private async Task<Model.PaymentTransaction> GenerateOrderAsync(Model.PaymentTransaction paymentTransaction, DateTime utcNow, CancellationToken cancellationToken = default)
    {
        string deeplinkTemplate = $"{_hostFe}{paymentTransaction.ReturnUrl}&orderCode={paymentTransaction.OrderCode}";
        var paymentGatewayConfig = await paymentGatewayService.GetConfigDataAsync(cancellationToken);
        var timeLimit = paymentGatewayService.GetTimeLimit(paymentTransaction.PaymentType, paymentGatewayConfig);

        // Gán dữ liệu cho payment-transaction
        paymentTransaction.ExpiredDatetimeUtc = utcNow.AddMinutes(timeLimit);
        paymentTransaction.ReturnUrl = deeplinkTemplate;

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
            OrderDescription = paymentGatewayConfig.Config?.OrderDescription,
            CustomerFullName = string.Empty,
            ReturnUrl = deeplinkTemplate,
            CancelUrl = deeplinkTemplate,
            AgainUrl = deeplinkTemplate,
            TypeCardAccount = paymentGatewayService.GetTypeCardAcount(paymentTransaction.PaymentType),
            TimeLimit = timeLimit,
            CustomerIdNumber = paymentTransaction.CustomerIdNumber,
            WalletFunctionType = paymentGatewayService.GetWalletFunctionType(paymentTransaction.PlatformType, paymentTransaction.PaymentType),
            TotalGoods = 1,
            DetailGoods = new List<PaymentGateway.Request.DetailInfo>()
            {
                new()
                {
                    GoodsCode = paymentTransaction.BillId,
                    GoodsName = paymentGatewayConfig.Config?.OrderDescription,
                    GoodsUrl = deeplinkTemplate,
                    GoodsQuantity = 1,
                    GoodsPrice = paymentTransaction.TotalAmount,
                }
            },
            AgencyCode = paymentGatewayConfig.Config?.AgencyCode
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

            // Xử lí check trans cho trường hợp chờ xử lí
            BackgroundJob.Schedule(() => CheckPaymentAsync(new()
            {
                BillId = paymentTransaction.BillId,
                OrderCode = paymentTransaction.OrderCode
            }, DateTime.UtcNow.ConvertUtcToVietnamTz(), cancellationToken), TimeSpan.FromMinutes(timeLimit + 2));
        }
        else
            paymentTransaction.PaymentProviderStatus = PaymentStatus.Fail;

        return paymentTransaction;
    }

    private Model.PaymentTransaction CreatePaymentTransaction(GenerateRequest request, Model.Device? device, DateTime utcNow)
    {
        Model.PaymentTransaction paymentTransaction = new()
        {
            TraceId = _httpContext != null ? _httpContext.TraceIdentifier : RelateText.GenId(),
            PaymentType = request.PaymentType,
            OrderCode = RelateText.GenId(),
            BillId = request.BillId!,
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
            DeviceCode = device?.Code,
            TotalAmount = request!.TotalAmount,
            PlatformType = request.PlatformType,
            Active = true,
            CreatedDatetimeUtc = utcNow,
            UpdatedDatetimeUtc = utcNow,
        };

        paymentTransaction.TransactionTrackings = new()
        {
            new()
            {
                TraceId = paymentTransaction.TraceId,
                ServiceProviderStatus = ServiceStatus.None,
                PaymentProviderStatus = PaymentStatus.None,
                Active = true,
                CreatedDatetimeUtc = utcNow,
                UpdatedDatetimeUtc = utcNow,
            }
        };

        return paymentTransaction;
    }

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