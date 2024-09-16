using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Payment.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Payment.Response;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.PaymentGateway.Request;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Services;

public interface IPaymentService : IBaseService
{
    /// <summary>
    /// Chức năng: xử lí IPN từ payment-gateway
    /// </summary>
    /// <param name="request"></param>
    /// <param name="utcNow"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task ProcessCallbackAsync(BaseRequest<string> request, DateTime utcNow, CancellationToken cancellationToken);
    
    /// <summary>
    /// Chức năng: kiểm tra trạng thái giao dịch
    /// </summary>
    /// <param name="request"></param>
    /// <param name="utcNow"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<CheckResponse>> CheckPaymentAsync(CheckRequest request, DateTime utcNow, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Chức năng: khởi tạo giao dịch thanh toán
    /// </summary>
    /// <param name="request"></param>
    /// <param name="utcNow"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<GenerateResponse>> GeneratePaymentAsync(GenerateRequest request, DateTime utcNow, CancellationToken cancellationToken = default);
}