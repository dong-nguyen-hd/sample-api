using EPAY.AIRWAY.KIOSK.API.Controllers.Config;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;

namespace EPAY.AIRWAY.KIOSK.API.Controllers;

[Route("api/v1/log")]
[ApiController]
[Authorize]
public sealed class LogController(ILogService logService, IMapper mapper) : ParentController(mapper)
{
    #region Action

    [Authorize(Policy = MyPolicy.Administrator)]
    [HttpGet("get-by-id/{id}")]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<Model.Log>), 200)]
    [SwaggerOperation(summary: "Lấy ra bản ghi log dựa trên Id")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] string id, CancellationToken cancellationToken)
    {
        var result = await logService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    #endregion
}