using EPAY.AIRWAY.KIOSK.API.Domain.Context;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Payment.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Payment.Response;
using EPAY.AIRWAY.KIOSK.API.Resources.Enums;
using EPAY.AIRWAY.KIOSK.API.Resources.Exceptions;
using Hangfire;
using IdGen;
using Microsoft.EntityFrameworkCore;
using PaymentGateway = EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.PaymentGateway;

namespace EPAY.AIRWAY.KIOSK.API.Services;

public sealed class PaymentService(
    ISignalRService signalRService,
    IConfigurationService configurationService,
    IHttpContextAccessor httpContextAccessor,
    IPaymentGatewayService paymentGatewayService,
    IMapper mapper,
    CoreContext context) : BaseService, IPaymentService
{
    #region Properties

    private readonly HttpContext? _httpContext = httpContextAccessor?.HttpContext;
    private string? _hostFe;

    #endregion

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

    #region Check Payment

    public async Task<BaseResult<CheckResponse>> CheckPaymentAsync(CheckRequest request, DateTime utcNow, CancellationToken cancellationToken = default)
    {
        await GetConfigDataAsync(cancellationToken);

        var paymentTransaction = await context.PaymentTransactions
            .SingleOrDefaultAsync(x => x.OrderCode == request.OrderCode && x.BillId == request.BillId, cancellationToken);

        // Validate data
        if (paymentTransaction == null)
            return GetBaseResult<CheckResponse>(CodeMessage._3005);

        // Gọi lại hàm kiểm tra giao dịch nếu trạng thái lúc này vẫn chưa kết thúc (successs, fail,...)
        if (paymentTransaction.ServiceProviderStatus != PaymentStatus.Success)
        {
            try
            {
                if (IsValid(paymentTransaction.PaymentProviderStatus))
                {
                    var paymentGatewayResult = await paymentGatewayService.CheckOrderAsync(new()
                    {
                        OrderCode = request.OrderCode
                    }, utcNow.ConvertUtcToVietnamTz(), cancellationToken);

                    paymentTransaction.PaymentProviderStatus = paymentGatewayResult.Data.PaymentStatus;

                    // Lấy ra giá trị phương thức thanh toán thực tế
                    var actualPaymentMethod = paymentGatewayResult.Data?.TransactionInfos?.FirstOrDefault();
                    if (actualPaymentMethod != null)
                    {
                        var actualType = actualPaymentMethod.PaymentMethod;
                        paymentTransaction.PartnerPaymentType = actualType != 0 ? actualType?.ToString("D2") : actualType.ToString();
                        paymentTransaction.TransCode = actualPaymentMethod.TransCode;
                    }
                }
            }
            catch
            {
                paymentTransaction.PaymentProviderStatus = PaymentStatus.Unknown;
            }

            try
            {
                if (paymentTransaction.PaymentProviderStatus == PaymentStatus.Success)
                {
                    int totalAmount = Convert.ToInt32(paymentTransaction.TotalAmount);

                    // var updateRailway = await _dsvnService.FinishPayment(new()
                    // {
                    //     PaymentId = data.PaymentId,
                    //     ThoiDiemKTGiaoDich = data.BookingExpiredDatetime,
                    //     MaDoiSoat = data.TransCode,
                    //     Amount = totalAmount,
                    //     SettlementAmount = totalAmount / 1000,
                    //     ChannelCode = data.PartnerPaymentType ?? string.Empty,
                    //     AdditionData = data.OrderCode,
                    //     ServiceCode = data.PartnerPaymentType == "03" ? _serviceCodeGlobal : _serviceCode
                    // });
                    //
                    // if (updateRailway.Status == ResultStatusEnum.Success)
                    //     data.RailwayPaymentStatus = PaymentStatus.Success;
                    // else
                    //     data.RailwayPaymentStatus = PaymentStatus.Fail;
                }
            }
            catch
            {
                paymentTransaction.ServiceProviderStatus = PaymentStatus.Unknown;
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
            context.Update(paymentTransaction);
            await context.SaveChangesAsync(cancellationToken);
        }

        // Mapping model
        var resource = mapper.Map<CheckResponse>(paymentTransaction);

        return GetBaseResult(CodeMessage._0000, data: resource);

        // Gọi lại hàm kiểm tra giao dịch nếu trạng thái lúc này vẫn chưa kết thúc (successs, fail,...)
        bool IsValid(PaymentStatus source)
        {
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
            return GetBaseResult<GenerateResponse>(CodeMessage._3005);

        // Process data payment-trans
        var paymentTransaction = CreatePaymentTransaction(request, utcNow);

        try
        {
            paymentTransaction = await GenerateOrderAsync(paymentTransaction, utcNow, cancellationToken);
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex.Message, ex);
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
            Serilog.Log.Error($"{ex.Message} >>>> {paymentTransaction.MySerialize()}", ex);

            throw;
        }

        // Mapping result
        var result = mapper.Map<GenerateResponse>(paymentTransaction);
        result.RequestDatetimeUtc = utcNow;

        // Process result
        if (paymentTransaction.PaymentProviderStatus == PaymentStatus.Success ||
            paymentTransaction.PaymentProviderStatus == PaymentStatus.Init) // Thành công
            return GetBaseResult(CodeMessage._0000, data: result);

        return GetBaseResult(CodeMessage._3005, data: result);
    }

    private async Task<Model.PaymentTransaction> GenerateOrderAsync(Model.PaymentTransaction paymentTransaction, DateTime utcNow, CancellationToken cancellationToken = default)
    {
        string deeplinkTemplate = $"{_hostFe}{paymentTransaction.ReturnUrl}&orderCode={paymentTransaction.OrderCode}";
        var paymentGatewayConfig = await paymentGatewayService.GetConfigDataAsync(cancellationToken);
        var totalAmount = Convert.ToInt32(paymentTransaction.TotalAmount);
        var timeLimit = paymentGatewayService.GetTimeLimit(paymentTransaction.PaymentType, paymentGatewayConfig);

        // Gán dữ liệu cho payment-transaction
        paymentTransaction.ExpiredDatetimeUtc = utcNow.AddMinutes(timeLimit);
        paymentTransaction.ReturnUrl = deeplinkTemplate;

        // Request to payment-gateway
        var paymentGatewayResult = await paymentGatewayService.CreateOrderAsync(new()
        {
            ChannelCode = paymentGatewayService.GetChannelCode(paymentTransaction.PlatformType, paymentTransaction.PaymentType),
            KioskId = paymentTransaction.DeviceId,
            PosSerial = paymentTransaction.PosSerial,
            PosRefId = paymentTransaction.PosRefId,
            PosMerchantId = paymentTransaction.PosMerchantId,
            PosClientId = paymentTransaction.PosClientId,
            PosMerchantOutletId = paymentTransaction.PosMerchantOutletId,
            PosTerminalId = paymentTransaction.PosTerminalId,
            PaymentMethod = paymentGatewayService.GetPaymentMethod(paymentTransaction.PaymentType),
            MerchantCode = paymentGatewayConfig.Config.MerchantCode,
            MerchantPassword = paymentGatewayConfig.Config.Password,
            OrderCode = paymentTransaction.OrderCode,
            BillId = paymentTransaction.BillId,
            PaymentType = 1,
            TotalAmount = totalAmount,
            OrderAmount = totalAmount,
            OrderDescription = paymentGatewayConfig.Config.OrderDescription,
            CustomerFullName = string.Empty,
            ReturnUrl = deeplinkTemplate,
            CancelUrl = deeplinkTemplate,
            AgainUrl = deeplinkTemplate,
            TypeCardAccount = paymentGatewayService.GetTypeCardAcount(paymentTransaction.PaymentType),
            TimeLimit = timeLimit,
            CustomerAddress = string.Empty, // TODO: bo sung luu ten
            TotalGoods = 1,
            DetailGoods = new List<PaymentGateway.Request.DetailInfo>()
            {
                new()
                {
                    GoodsCode = paymentTransaction.BillId,
                    GoodsName = paymentGatewayConfig.Config.OrderDescription,
                    GoodsUrl = deeplinkTemplate,
                    GoodsQuantity = 1,
                    GoodsPrice = totalAmount,
                }
            },
            AgencyCode = paymentGatewayConfig.Config.AgencyCode
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
            if (paymentTransaction.PaymentType == PaymentType.QR ||
                paymentTransaction.PaymentType == PaymentType.LocalCard ||
                paymentTransaction.PaymentType == PaymentType.GlobalCard ||
                paymentTransaction.PaymentType == PaymentType.BankAccount)
            {
                BackgroundJob.Schedule(() => CheckPaymentAsync(new()
                {
                    BillId = paymentTransaction.BillId,
                    OrderCode = paymentTransaction.OrderCode
                }, DateTime.UtcNow.ConvertUtcToVietnamTz(), cancellationToken), TimeSpan.FromMinutes(timeLimit + 2));
            }
        }
        else
            paymentTransaction.PaymentProviderStatus = PaymentStatus.Fail;

        return paymentTransaction;
    }

    private Model.PaymentTransaction CreatePaymentTransaction(GenerateRequest request, DateTime utcNow)
    {
        Model.PaymentTransaction paymentTransaction = new()
        {
            TraceId = _httpContext != null ? _httpContext.TraceIdentifier : Guid.NewGuid().ToString(),
            PaymentType = request.PaymentType,
            OrderCode = RelateText.GenId(),
            BillId = request.BillId,
            IdNumber = request.IdNumber,
            ServiceProviderStatus = PaymentStatus.None,
            PaymentProviderStatus = PaymentStatus.None,
            ReturnUrl = request.ReturnUrl,
            PosSerial = request.PosSerial,
            PosRefId = request.PosRefId,
            PosMerchantId = request.PosMerchantId,
            PosClientId = request.PosClientId,
            PosMerchantOutletId = request.PosMerchantOutletId,
            PosTerminalId = request.PosTerminalId,
            DeviceId = string.Empty,
            TotalAmount = request.TotalAmount,
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
                ServiceProviderStatus = PaymentStatus.None,
                PaymentProviderStatus = PaymentStatus.None,
                Active = true,
                CreatedDatetimeUtc = utcNow,
                UpdatedDatetimeUtc = utcNow,
            }
        };

        return paymentTransaction;
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