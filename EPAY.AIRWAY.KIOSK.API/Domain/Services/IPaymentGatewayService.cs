using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.PaymentGateway.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.PaymentGateway.Response;
using EPAY.AIRWAY.KIOSK.API.Resources.Enums;
using EPAY.AIRWAY.KIOSK.API.Resources.SystemData.ThirdParty.PaymentGateway;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Services;

public interface IPaymentGatewayService : IBaseService
{
    /// <summary>
    /// Chức năng: lấy ra cấu hình payment-gateway
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PaymentGatewayInfo> GetConfigDataAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Chức năng: giải mã bản tin Callback
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<CallbackRequest>> DecryptDataCallBackAsync(BaseRequest<string> request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: lấy access-token
    /// </summary>
    /// <param name="request"></param>
    /// <param name="now"></param>
    /// <param name="paymentGatewayInfo"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<LoginResponse>> GetTokenAsync(LoginRequest request, DateTime now, PaymentGatewayInfo? paymentGatewayInfo, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: tạo đơn hàng
    /// </summary>
    /// <param name="request"></param>
    /// <param name="now"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<CreateOrderResponse>> CreateOrderAsync(CreateOrderRequest request, DateTime now, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: kiểm tra trạng thái đơn hàng
    /// </summary>
    /// <param name="request"></param>
    /// <param name="now"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<CheckOrderResponse>> CheckOrderAsync(CheckOrderRequest request, DateTime now, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: khởi tạo giao dịch hoàn tiền
    /// </summary>
    /// <param name="request"></param>
    /// <param name="now"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<RefundResponse>> RefundAsync(RefundRequest request, DateTime now, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: get channel code
    /// </summary>
    /// <param name="platformType"></param>
    /// <param name="paymentType"></param>
    /// <returns></returns>
    string GetChannelCode(PlatformType platformType, PaymentType paymentType);

    /// <summary>
    /// Chức năng: get payment method
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    string GetPaymentMethod(PaymentType request);

    /// <summary>
    /// Chức năng: get type card account
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    string? GetTypeCardAcount(PaymentType request);

    /// <summary>
    /// Chức năng; get time limit
    /// </summary>
    /// <param name="request"></param>
    /// <param name="paymentGatewayInfo"></param>
    /// <returns></returns>
    int GetTimeLimit(PaymentType request, PaymentGatewayInfo paymentGatewayInfo);
}