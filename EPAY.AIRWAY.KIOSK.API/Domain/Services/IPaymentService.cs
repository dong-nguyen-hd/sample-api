using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Payment.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Payment.Response;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.PaymentGateway.Request;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Services;

public interface IPaymentService : IBaseService
{
    Task ProcessCallbackAsync(BaseRequest<string> request, CancellationToken cancellationToken);
    Task<BaseResult<CheckResponse>> CheckPaymentAsync(CheckRequest request, DateTime utcNow, CancellationToken cancellationToken = default);
    Task<BaseResult<GenerateResponse>> GeneratePaymentAsync(GenerateRequest request, DateTime utcNow, CancellationToken cancellationToken = default);
}