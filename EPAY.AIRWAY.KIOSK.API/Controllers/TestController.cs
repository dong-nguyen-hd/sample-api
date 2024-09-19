using EPAY.AIRWAY.KIOSK.API.Controllers.Config;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Payment.Request;
using Microsoft.AspNetCore.Http.Timeouts;

namespace EPAY.AIRWAY.KIOSK.API.Controllers;

[Route("api/v1/test")]
[ApiController]
[Authorize]
public sealed class TestController(ISignalRService signalRService) : ParentController
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
        if(!SystemGlobal.IsDebug)
            return GetBaseResult<object>(404, null);
        
        await signalRService.PublicMessageAsync(request, cancellationToken);

        return GetBaseResult<string>(200, "Success");
    }

    #endregion
}