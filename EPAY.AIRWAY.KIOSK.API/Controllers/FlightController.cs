using EPAY.AIRWAY.KIOSK.API.Controllers.Config;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;

namespace EPAY.AIRWAY.KIOSK.API.Controllers;

[Route("api/v1/flight")]
[ApiController]
[Authorize]
public sealed class FlightController(IFlightService flightService, IMapper mapper) : ParentController(mapper)
{
    #region Action

    [Authorize(Policy = MyPolicy.Device)]
    [HttpPost("find")]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<SearchResponse>), 200)]
    [SwaggerOperation(summary: "Lấy ra thông tin chuyến bay")]
    public async Task<IActionResult> SearchTempAsync([FromBody] SearchRequest request, CancellationToken cancellationToken)
    {
        var result = await flightService.SearchAsync(request, cancellationToken);
        return Ok(result);
    }

    [Authorize(Policy = MyPolicy.Device)]
    [HttpGet("master-data")]
    [ResponseCache(CacheProfileName = CustomCacheProfile.Any5m)]
    [ProducesResponseType(typeof(BaseResult<MasterDataResponse>), 200)]
    [SwaggerOperation(summary: "Lấy ra thông tin model, hãng bay, cảng hảng không")]
    public async Task<IActionResult> GetMasterDataAsync(CancellationToken cancellationToken)
    {
        var result = await flightService.GetMasterDataAsync(true, cancellationToken);

        if (result.CodeMessage == CodeMessage._0000)
        {
            result.Data!.Aircrafts = default;
            result.Data!.Airlines = default;
        }

        return Ok(result);
    }

    [Authorize(Policy = MyPolicy.Device)]
    [HttpPost("additional-services")]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<AdditionalServicesResponse>), 200)]
    [SwaggerOperation(summary: "Lấy ra thông tin hành lí, dịch vụ mua thêm")]
    public async Task<IActionResult> AdditionalServicesAsync([FromBody] AdditionalServicesRequest request, CancellationToken cancellationToken)
    {
        var result = await flightService.GetAdditionalServicesAsync(request, cancellationToken);
        return Ok(result);
    }

    [Authorize(Policy = MyPolicy.Device)]
    [HttpPost("verify")]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<VerifyResponse>), 200)]
    [SwaggerOperation(summary: "Kiểm tra thông tin chuyến bay")]
    public async Task<IActionResult> VerifyAsync([FromBody] VerifyRequest request, CancellationToken cancellationToken)
    {
        var result = await flightService.VerifyAsync(request, cancellationToken);
        return Ok(result);
    }

    [Authorize(Policy = MyPolicy.Device)]
    [HttpPost("booking")]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<BookingResponse>), 200)]
    [SwaggerOperation(summary: "Lưu thông tin hành khách và đặt chỗ")]
    public async Task<IActionResult> BookingAsync([FromBody] BookingRequest request, CancellationToken cancellationToken)
    {
        var result = await flightService.BookingAsync(request, cancellationToken);
        return Ok(result);
    }

    #endregion
}