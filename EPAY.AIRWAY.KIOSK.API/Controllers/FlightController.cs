using EPAY.AIRWAY.KIOSK.API.Controllers.Config;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;

namespace EPAY.AIRWAY.KIOSK.API.Controllers;

[Route("api/v1/flight")]
[ApiController]
//[Authorize]
public sealed class FlightController(IFlightService flightService, IMapper mapper) : ParentController(mapper)
{
    #region Action

    //[Authorize(Policy = MyPolicy.Administrator)]
    [HttpPost("find")]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<SearchResponse>), 200)]
    [SwaggerOperation(summary: "Lấy ra thông tin chuyến bay")]
    public async Task<IActionResult> SearchAsync([FromBody] SearchRequest request, CancellationToken cancellationToken)
    {
        var result = await flightService.SearchAsync(request, cancellationToken);
        return Ok(result);
    }

    #endregion
}