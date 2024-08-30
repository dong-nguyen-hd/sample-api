using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.CustomHttpClient.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.PaymentGateway.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.PaymentGateway.Response;
using EPAY.AIRWAY.KIOSK.API.Resources.Enums;
using EPAY.AIRWAY.KIOSK.API.Resources.Exceptions;
using EPAY.AIRWAY.KIOSK.API.Resources.SystemData.ThirdParty.PaymentGateway;

namespace EPAY.AIRWAY.KIOSK.API.Services.ThirdParty;

public sealed class PaymentGatewayService(
    IConfigurationService configurationService,
    ICustomHttpClient customHttpClient) : BaseService, IPaymentGatewayService
{
    #region Properties

    private PaymentGatewayInfo? _paymentGatewayInfo;

    #endregion

    #region Method

    public async Task<BaseResult<CallbackRequest>> DecryptDataCallBackAsync(BaseRequest<string> request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(request.Data))
            return GetBaseResult<CallbackRequest>(CodeMessage._3005);

        // Get config
        var info = await GetConfigDataAsync(cancellationToken);

        var decryptData = request.Data.DecryptDataForPaymentGateway<CallbackRequest>(info.Config?.SecretKey!);

        // Process result
        if (decryptData != null)
            return GetBaseResult(CodeMessage._0000, decryptData);

        return GetBaseResult<CallbackRequest>(CodeMessage._3005);
    }

    public async Task<BaseResult<LoginResponse>> GetTokenAsync(LoginRequest request, DateTime now, PaymentGatewayInfo? paymentGatewayInfo, CancellationToken cancellationToken = default)
    {
        // Get config
        var info = paymentGatewayInfo ?? await GetConfigDataAsync(cancellationToken);

        // Request to 3th
        var payload = request.EncryptedDataForPaymentGateway(now, info.Config?.MerchantCode!, info.Config?.SecretKey!, info.Config?.PrivateKeyForBe!);
        var baseResponse = await customHttpClient.SendAsync(new MyHttpRequest
        {
            Uri = new Uri(info.Api?.GetLoginUri()!),
            Payload = payload.MySerialize(),
            MyHttpMethod = MyHttpMethod.POST,
            NumberRetry = 2,
            EnableVerifyTls = info.Api!.EnableVerifyTls,
            Headers = GetHeaderRequest(info)
        }, ProcessResult<BaseResponse<string>>, CodeMessage._3005, cancellationToken);

        // Process result
        if (baseResponse.codeMessage != CodeMessage._0000)
            return GetBaseResult<LoginResponse>(CodeMessage._3005);

        var loginResponse = baseResponse.data?.Data?.DecryptDataForPaymentGateway<BaseResponse<LoginResponse>>(info.Config?.SecretKey!);

        if (loginResponse?.Data?.ErrorCode == 0 && !string.IsNullOrEmpty(loginResponse.Data.Token)) // 0: là mã thành công phía payment-gateway
            return GetBaseResult(CodeMessage._0000, loginResponse.Data);

        return GetBaseResult<LoginResponse>(CodeMessage._3005);
    }

    public async Task<BaseResult<CreateOrderResponse>> CreateOrderAsync(CreateOrderRequest request, DateTime now, CancellationToken cancellationToken = default)
    {
        // Get config
        var info = await GetConfigDataAsync(cancellationToken);

        // Get token
        var tokenResponse = await GetTokenAsync(new LoginRequest()
        {
            UserName = info.Config?.Account,
            Password = info.Config?.Password,
        }, now, info, cancellationToken);

        // Request to 3th
        var payload = request.EncryptedDataForPaymentGateway(now, info.Config?.MerchantCode!, info.Config?.SecretKey!, info.Config?.PrivateKeyForBe!);
        var baseResponse = await customHttpClient.SendAsync(new MyHttpRequest
        {
            Uri = new Uri(info.Api?.GetCreateOrderUri()!),
            Payload = payload.MySerialize(),
            MyHttpMethod = MyHttpMethod.POST,
            NumberRetry = 0,
            EnableVerifyTls = info.Api!.EnableVerifyTls,
            Headers = GetHeaderRequest(info, tokenResponse.Data!.Token!)
        }, ProcessResult<BaseResponse<string>>, CodeMessage._3005, cancellationToken);

        // Process result
        if (baseResponse.codeMessage != CodeMessage._0000)
            return GetBaseResult<CreateOrderResponse>(CodeMessage._3005);

        var createOrderResponse = baseResponse.data!.Data!.DecryptDataForPaymentGateway<BaseResponse<CreateOrderResponse>>(info.Config?.SecretKey!);

        if (createOrderResponse.Data!.ErrorCode == 0) // 0: là mã thành công phía payment-gateway
            return GetBaseResult(CodeMessage._0000, createOrderResponse.Data);

        return GetBaseResult<CreateOrderResponse>(CodeMessage._3005);
    }

    public async Task<BaseResult<CheckOrderResponse>> CheckOrderAsync(CheckOrderRequest request, DateTime now, CancellationToken cancellationToken = default)
    {
        // Get config
        var info = await GetConfigDataAsync(cancellationToken);

        // Get token
        var tokenResponse = await GetTokenAsync(new LoginRequest()
        {
            UserName = info.Config?.Account,
            Password = info.Config?.Password,
        }, now, info, cancellationToken);

        // Request to 3th
        var payload = request.EncryptedDataForPaymentGateway(now, info.Config?.MerchantCode!, info.Config?.SecretKey!, info.Config?.PrivateKeyForBe!);
        var baseResponse = await customHttpClient.SendAsync(new MyHttpRequest
        {
            Uri = new Uri(info.Api?.GetCheckStatusUri()!),
            Payload = payload.MySerialize(),
            MyHttpMethod = MyHttpMethod.POST,
            NumberRetry = 2,
            EnableVerifyTls = info.Api!.EnableVerifyTls,
            Headers = GetHeaderRequest(info, tokenResponse.Data!.Token!)
        }, ProcessResult<BaseResponse<string>>, CodeMessage._3005, cancellationToken);

        // Process result
        if (baseResponse.codeMessage != CodeMessage._0000)
            return GetBaseResult<CheckOrderResponse>(CodeMessage._3005);

        var checkOrderResponse = baseResponse!.data!.Data!.DecryptDataForPaymentGateway<BaseResponse<CheckOrderResponse>>(info.Config?.SecretKey!);

        // Mappnig payment-status from PaymentGateway to BE
        var paymentStatus = MappingPaymentStatus(checkOrderResponse);
        if (checkOrderResponse.Data != null)
            checkOrderResponse.Data.PaymentStatus = paymentStatus;
        else
            checkOrderResponse.Data = new() { PaymentStatus = paymentStatus };

        if (checkOrderResponse.Data.ErrorCode == 0) // 0: là mã thành công phía payment-gateway
            return GetBaseResult(CodeMessage._0000, checkOrderResponse.Data);

        return GetBaseResult<CheckOrderResponse>(CodeMessage._3005);
    }

    public async Task<BaseResult<RefundResponse>> RefundAsync(RefundRequest request, DateTime now, CancellationToken cancellationToken = default)
    {
        // Get config
        var info = await GetConfigDataAsync(cancellationToken);

        // Get token
        var tokenResponse = await GetTokenAsync(new LoginRequest()
        {
            UserName = info.Config?.Account,
            Password = info.Config?.Password,
        }, now, info, cancellationToken);

        // Request to 3th
        var payload = request.EncryptedDataForPaymentGateway(now, info.Config?.MerchantCode!, info.Config?.SecretKey!, info.Config?.PrivateKeyForBe!);
        var baseResponse = await customHttpClient.SendAsync(new MyHttpRequest
        {
            Uri = new Uri(info.Api!.GetRefundUri()),
            Payload = payload.MySerialize(),
            MyHttpMethod = MyHttpMethod.POST,
            NumberRetry = 0,
            EnableVerifyTls = info.Api.EnableVerifyTls,
            Headers = GetHeaderRequest(info, tokenResponse.Data!.Token!)
        }, ProcessResult<BaseResponse<string>>, CodeMessage._3005, cancellationToken);

        // Process result
        if (baseResponse.codeMessage != CodeMessage._0000)
            return GetBaseResult<RefundResponse>(CodeMessage._3005);

        var refundResponse = baseResponse.data!.Data!.DecryptDataForPaymentGateway<BaseResponse<RefundResponse>>(info.Config?.SecretKey!);

        if (refundResponse.Data!.ErrorCode == 0) // 0: là mã thành công phía payment-gateway
            return GetBaseResult(CodeMessage._0000, refundResponse.Data);

        return GetBaseResult<RefundResponse>(CodeMessage._3005);
    }

    public string GetChannelCode(PlatformType platformType, PaymentType paymentType)
    {
        switch (GetPaymentChannel(platformType, paymentType))
        {
            case PaymentChannel.Website:
                return "01";
            case PaymentChannel.MobileApp:
                return "02";
            case PaymentChannel.Pos:
                return "03";
            case PaymentChannel.SmartPos:
                return "04";
            case PaymentChannel.QrStatic:
                return "05";
            case PaymentChannel.Kiosk:
                return "06";
            case PaymentChannel.MiniKiosk:
                return "07";
            case PaymentChannel.SmartGate:
                return "08";
            default:
                throw new MessageResultException("Không tìm thấy loại thanh toán phù hợp");
        }
    }

    public string GetPaymentMethod(PaymentType request)
    {
        switch (request)
        {
            case PaymentType.EpayWallet:
                return "01";
            case PaymentType.QR:
                return "04";
            case PaymentType.POS:
                return "00";
            case PaymentType.LocalCard:
            case PaymentType.BankAccount:
                return "02";
            case PaymentType.GlobalCard:
                return "03";
            default:
                throw new MessageResultException("Không tìm thấy loại thanh toán phù hợp");
        }
    }

    public string? GetTypeCardAcount(PaymentType request)
    {
        switch (request)
        {
            case PaymentType.LocalCard:
                return "Card";
            case PaymentType.BankAccount:
                return "Account";
            default:
                return null;
        }
    }

    public int GetTimeLimit(PaymentType request, PaymentGatewayInfo paymentGatewayInfo)
    {
        switch (request)
        {
            case PaymentType.QR:
                return (int)paymentGatewayInfo.Config?.TimeLimitQr!;
            case PaymentType.LocalCard:
            case PaymentType.GlobalCard:
                return (int)paymentGatewayInfo.Config?.TimeLimitCard!;
            case PaymentType.BankAccount:
                return (int)paymentGatewayInfo.Config?.TimeLimitBankAccount!;
            default:
                throw new MessageResultException("Không tìm thấy loại thanh toán phù hợp");
        }
    }

    public async Task<PaymentGatewayInfo> GetConfigDataAsync(CancellationToken cancellationToken = default)
    {
        // Sử dụng lại config đã lấy ra trước đó nếu có dữ liệu
        if (_paymentGatewayInfo != null)
            return _paymentGatewayInfo with { };

        // Get config from DB
        var configurations = await configurationService.GetAllAsync(false, cancellationToken);

        if (configurations.CodeMessage != CodeMessage._0000)
            throw new MessageResultException("Không thể thực hiện lấy config");

        // Process result
        PaymentGatewayInfo info = new()
        {
            Config = new(),
            Api = new()
        };

        foreach (var configuration in configurations.Data)
        {
            // Config
            if (configuration.Key == SystemConfig.PaymentGatewayMerchantCode)
            {
                info.Config.MerchantCode = configuration.Value;
                continue;
            }

            if (configuration.Key == SystemConfig.PaymentGatewayMerchantCode)
            {
                info.Config.Account = configuration.Value;
                continue;
            }
            
            if (configuration.Key == SystemConfig.PaymentGatewayAccount)
            {
                info.Config.Account = configuration.Value;
                continue;
            }

            if (configuration.Key == SystemConfig.PaymentGatewayPassword)
            {
                info.Config.Password = configuration.Value;
                continue;
            }

            if (configuration.Key == SystemConfig.PaymentGatewayPublicKeyForBe)
            {
                info.Config.PublicKeyForBe = configuration.Value;
                continue;
            }

            if (configuration.Key == SystemConfig.PaymentGatewayPrivateKeyForBe)
            {
                info.Config.PrivateKeyForBe = configuration.Value;
                continue;
            }

            if (configuration.Key == SystemConfig.PaymentGatewayPublicKey)
            {
                info.Config.PublicKey = configuration.Value;
                continue;
            }

            if (configuration.Key == SystemConfig.PaymentGatewaySecretKey)
            {
                info.Config.SecretKey = configuration.Value;
                continue;
            }

            if (configuration.Key == SystemConfig.PaymentGatewayClientIp)
            {
                info.Config.ClientIp = configuration.Value;
                continue;
            }

            if (configuration.Key == SystemConfig.PaymentGatewayTimeLimitQr)
            {
                info.Config.TimeLimitQr = int.Parse(configuration.Value!);
                continue;
            }

            if (configuration.Key == SystemConfig.PaymentGatewayTimeLimitCard)
            {
                info.Config.TimeLimitCard = int.Parse(configuration.Value!);
                continue;
            }

            if (configuration.Key == SystemConfig.PaymentGatewayTimeLimitBankAccount)
            {
                info.Config.TimeLimitBankAccount = int.Parse(configuration.Value!);
                continue;
            }

            if (configuration.Key == SystemConfig.PaymentGatewayOrderDescription)
            {
                info.Config.OrderDescription = configuration.Value;
                continue;
            }

            if (configuration.Key == SystemConfig.PaymentGatewayAgencyCode)
            {
                info.Config.AgencyCode = configuration.Value;
                continue;
            }

            // Api
            if (configuration.Key == SystemConfig.PaymentGatewayEnableVerifyTls)
            {
                info.Api.EnableVerifyTls = bool.Parse(configuration.Value!);
                continue;
            }

            if (configuration.Key == SystemConfig.PaymentGatewayBaseAddress)
            {
                info.Api.BaseAddress = configuration.Value;
                continue;
            }

            if (configuration.Key == SystemConfig.PaymentGatewayRefund)
            {
                info.Api.Refund = configuration.Value;
                continue;
            }

            if (configuration.Key == SystemConfig.PaymentGatewayCreateOrder)
            {
                info.Api.CreateOrder = configuration.Value;
                continue;
            }

            if (configuration.Key == SystemConfig.PaymentGatewayCheckStatus)
            {
                info.Api.CheckStatus = configuration.Value;
                continue;
            }

            if (configuration.Key == SystemConfig.PaymentGatewayLogin)
            {
                info.Api.Login = configuration.Value;
                continue;
            }
        }

        _paymentGatewayInfo = info;

        return info;
    }

    #region Private work

    private static (CodeMessage, TRes?) ProcessResult<TRes>(HttpResponseMessage resource, string rawPayload)
    {
        if (resource.IsSuccessStatusCode && !string.IsNullOrEmpty(rawPayload))
            return (CodeMessage._0000, JsonSerializer.Deserialize<TRes>(rawPayload));

        return (CodeMessage._3005, default);
    }

    private static PaymentChannel GetPaymentChannel(PlatformType platformType, PaymentType paymentType)
    {
        if (platformType == PlatformType.Kiosk)
            return PaymentChannel.Kiosk;

        return PaymentChannel.Website;
    }

    private List<HeaderRequest> GetHeaderRequest(PaymentGatewayInfo paymentGatewayInfo, string accessToken = "")
    {
        List<HeaderRequest> headerRequests = new()
        {
            new HeaderRequest
            {
                Key = "merchantCode",
                Value = paymentGatewayInfo.Config?.MerchantCode
            },
            new HeaderRequest
            {
                Key = "lang",
                Value = "vi"
            },
            new HeaderRequest
            {
                Key = "version",
                Value = "1.0.0"
            },
            new HeaderRequest
            {
                Key = "clientIp",
                Value = paymentGatewayInfo.Config?.ClientIp
            }
        };

        if (!string.IsNullOrEmpty(accessToken))
            headerRequests.Add(new()
            {
                Key = "Authorization",
                Value = $"Bearer {accessToken}"
            });

        return headerRequests;
    }

    private static PaymentStatus MappingPaymentStatus(BaseResponse<CheckOrderResponse> request)
    {
        // Mapping dựa vào mã lỗi
        if (request.Data!.ErrorCode == 60)
            return PaymentStatus.Init;

        var status = request.Data?.TransactionInfos?.FirstOrDefault();
        if (status == null)
            return PaymentStatus.Init;

        switch (status.TransStatus)
        {
            case 0:
                return PaymentStatus.Init;
            case 1:
                return PaymentStatus.Success;
            case 2:
                return PaymentStatus.Fail;
            case 3:
                return PaymentStatus.Pending;
            case 4:
                return PaymentStatus.Cancel;
            case 5:
                return PaymentStatus.Pending;
            default:
                return PaymentStatus.Unknown;
        }
    }

    #endregion

    #endregion
}