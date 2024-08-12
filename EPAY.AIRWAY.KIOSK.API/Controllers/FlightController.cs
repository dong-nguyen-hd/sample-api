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
    [ProducesResponseType(typeof(BaseResult<SearchResponseTemp>), 200)]
    [SwaggerOperation(summary: "Lấy ra thông tin chuyến bay")]
    public async Task<IActionResult> SearchTempAsync([FromBody] SearchRequest request, CancellationToken cancellationToken)
    {
        var result = await flightService.SearchTempAsync(request, cancellationToken);
        return Ok(result);
    }

    //[Authorize(Policy = MyPolicy.Device)]
    [HttpGet("master-data")]
    [ResponseCache(CacheProfileName = CustomCacheProfile.Any5m)]
    [ProducesResponseType(typeof(BaseResult<MasterDataResponse>), 200)]
    [SwaggerOperation(summary: "Lấy ra thông tin model, hãng bay, cảng hảng không")]
    public async Task<IActionResult> GetMasterDataAsync(CancellationToken cancellationToken)
    {
        var result = await flightService.GetMasterDataAsync(cancellationToken);

        if (result.CodeMessage == CodeMessage._99)
        {
            result.Data!.Aircrafts = default;
            result.Data!.Airlines = default;
        }
        
        return Ok(result);
    }

    #endregion
}