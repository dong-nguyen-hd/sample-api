using EPAY.AIRWAY.KIOSK.API.Controllers.Config;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Payment.Request;
using Microsoft.AspNetCore.Http.Timeouts;

namespace EPAY.AIRWAY.KIOSK.API.Controllers;

[Route("api/v1/test")]
[ApiController]
[Authorize]
public sealed class TestController(ISignalRService signalRService, IPaymentGatewayService paymentGatewayService) : ParentController
{
    #region Action

    [Authorize(Policy = MyPolicy.Device)]
    [HttpPost("signalr")]
    [RequestTimeout(CustomTimeoutProfile.Over15S)]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<object>), 200)]
    [SwaggerOperation(summary: "Test SignalR")]
    public async Task<IActionResult> PublicMessageAsync([FromBody] CheckRequest request, CancellationToken cancellationToken = default)
    {
        if (!SystemGlobal.IsDebug)
            return GetBaseResult<object>(404, null);

        await signalRService.PublicMessageAsync(request, cancellationToken);

        return GetBaseResult<string>(200, "Success");
    }

    [Authorize(Policy = MyPolicy.Administrator)]
    [HttpPost("my-encrypt")]
    [RequestTimeout(CustomTimeoutProfile.Over15S)]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<object>), 200)]
    [SwaggerOperation(summary: "My Encrypt")]
    public IActionResult MyAesEncrypt([FromForm] string plainText, [FromForm] string key)
    {
        if (!SystemGlobal.IsDebug)
            return GetBaseResult<object>(404, null);

        return GetBaseResult(200, plainText.MyAesEncrypt(key));
    }
    
    [Authorize(Policy = MyPolicy.Administrator)]
    [HttpPost("my-decrypt")]
    [RequestTimeout(CustomTimeoutProfile.Over15S)]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<object>), 200)]
    [SwaggerOperation(summary: "My Decrypt")]
    public IActionResult MyAesDecrypt([FromForm] string cipherText, [FromForm] string key)
    {
        if (!SystemGlobal.IsDebug)
            return GetBaseResult<object>(404, null);

        return GetBaseResult(200, cipherText.MyAesDecrypt(key));
    }
    
    [Authorize(Policy = MyPolicy.Device)]
    [HttpPost("payment-gateway-encrypt")]
    [RequestTimeout(CustomTimeoutProfile.Over15S)]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<object>), 200)]
    [SwaggerOperation(summary: "Payment Gateway Encrypt")]
    public IActionResult PaymentGatewayAesEncrypt([FromForm] string plainText, [FromForm] string key)
    {
        if (!SystemGlobal.IsDebug)
            return GetBaseResult<object>(404, null);

        return GetBaseResult(200, plainText.AesEncrypt(key));
    }
    
    [Authorize(Policy = MyPolicy.Device)]
    [HttpPost("payment-gateway-decrypt")]
    [RequestTimeout(CustomTimeoutProfile.Over15S)]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<object>), 200)]
    [SwaggerOperation(summary: "Payment Gateway Decrypt")]
    public IActionResult PaymentGatewayAesDecrypt([FromForm] string cipherText, [FromForm] string key)
    {
        if (!SystemGlobal.IsDebug)
            return GetBaseResult<object>(404, null);

        return GetBaseResult(200, cipherText.AesDecrypt(key));
    }

    #endregion
}