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

    //[Authorize(Policy = MyPolicy.Device)]
    [HttpPost("find")]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<SearchResponse>), 200)]
    [SwaggerOperation(summary: "Lấy ra thông tin chuyến bay")]
    public async Task<IActionResult> SearchAsync([FromBody] SearchRequest request, CancellationToken cancellationToken)
    {
        var result = await flightService.SearchAsync(request, cancellationToken);
        return Ok(result);
    }

    //[Authorize(Policy = MyPolicy.Device)]
    [HttpGet("aircrafts")]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<List<AircraftsResponse>>), 200)]
    [SwaggerOperation(summary: "Lấy ra thông tin model máy bay")]
    public async Task<IActionResult> GetAircraftsAsync(CancellationToken cancellationToken)
    {
        var result = await flightService.GetAircraftsAsync(cancellationToken);
        return Ok(result);
    }

    //[Authorize(Policy = MyPolicy.Device)]
    [HttpGet("airlines")]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<List<AirlinesResponse>>), 200)]
    [SwaggerOperation(summary: "Lấy ra thông tin hãng bay")]
    public async Task<IActionResult> GetAirlinesAsync(CancellationToken cancellationToken)
    {
        var result = await flightService.GetAirlinesAsync(cancellationToken);
        return Ok(result);
    }

    //[Authorize(Policy = MyPolicy.Device)]
    [HttpGet("airports")]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<List<AirportsResponse>>), 200)]
    [SwaggerOperation(summary: "Lấy ra thông tin cảng hàng không")]
    public async Task<IActionResult> GetAirportsAsync(CancellationToken cancellationToken)
    {
        var result = await flightService.GetAirportsAsync(cancellationToken);
        return Ok(result);
    }

    #endregion
}