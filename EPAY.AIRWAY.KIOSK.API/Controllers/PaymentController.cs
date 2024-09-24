using EPAY.AIRWAY.KIOSK.API.Controllers.Config;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Payment.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Payment.Response;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.PaymentGateway.Request;
using FluentValidation;
using Microsoft.AspNetCore.Http.Timeouts;

namespace EPAY.AIRWAY.KIOSK.API.Controllers;

[Route("api/v1/payment")]
[ApiController]
[Authorize]
public sealed class PaymentController(IPaymentService paymentService, IFlightService flightService) : ParentController
{
    #region Action

    [Authorize(Policy = MyPolicy.Device)]
    [HttpPost("create")]
    [RequestTimeout(CustomTimeoutProfile.Over15S)]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<GenerateResponse>), 200)]
    [SwaggerOperation(summary: "Khởi tạo thông tin giao dịch")]
    public async Task<IActionResult> GeneratePaymentAsync([FromBody] GenerateRequest request, [FromServices] IValidator<GenerateRequest> validator, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var result = await paymentService.GeneratePaymentAsync(request, DateTime.UtcNow, cancellationToken);
        return GetBaseResult(200, result);
    }

    [Authorize(Policy = MyPolicy.Device)]
    [HttpPost("check")]
    [RequestTimeout(CustomTimeoutProfile.Over15S)]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<CheckResponse>), 200)]
    [SwaggerOperation(summary: "Kiểm tra thông tin giao dịch")]
    public async Task<IActionResult> CheckPaymentAsync([FromBody] CheckRequest request, [FromServices] IValidator<CheckRequest> validator, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var result = await paymentService.CheckPaymentAsync(request, DateTime.UtcNow, cancellationToken);
        return GetBaseResult(200, result);
    }
    
    [Authorize(Policy = MyPolicy.Device)]
    [HttpPost("check-paylater")]
    [RequestTimeout(CustomTimeoutProfile.Over15S)]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<CheckOrderInfoResponse>), 200)]
    [SwaggerOperation(summary: "Kiểm tra thông tin giao dịch trả sau có tồn tại?")]
    public async Task<IActionResult> CheckPaymentAsync([FromBody] CheckOrderInfoRequest request, [FromServices] IValidator<CheckOrderInfoRequest> validator, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var result = await flightService.CheckOrderInfoAsync(request, DateTime.UtcNow, cancellationToken);
        return GetBaseResult(200, result);
    }

    [AllowAnonymous]
    [HttpPost("callback")]
    [RequestTimeout(CustomTimeoutProfile.Over1M)]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(200)]
    [SwaggerOperation(summary: "Xử lí IPN từ cổng thanh toán")]
    public async Task<IActionResult> ProcessCallbackAsync([FromBody] BaseRequest<string> request, CancellationToken cancellationToken)
    {
        await paymentService.ProcessCallbackAsync(request, DateTime.UtcNow, cancellationToken);
        return GetBaseResult<object>(200, null);
    }

    #endregion
}