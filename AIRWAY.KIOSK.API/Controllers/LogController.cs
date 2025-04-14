using AIRWAY.KIOSK.API.Controllers.Config;
using AIRWAY.KIOSK.API.Domain.Services;
using Microsoft.AspNetCore.Http.Timeouts;

namespace AIRWAY.KIOSK.API.Controllers;

[Route("api/v1/log")]
[ApiController]
[Authorize]
public sealed class LogController(ILogService logService) : ParentController
{
    #region Action

    [Authorize(Policy = MyPolicy.Administrator)]
    [HttpGet("get-by-id/{id}")]
    [RequestTimeout(CustomTimeoutProfile.Over15S)]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<Model.Log>), 200)]
    [SwaggerOperation(summary: "Lấy ra bản ghi log dựa trên Id")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] string id, CancellationToken cancellationToken)
    {
        var result = await logService.GetByIdAsync(id, cancellationToken);
        return GetBaseResult(200, result);
    }

    #endregion
}