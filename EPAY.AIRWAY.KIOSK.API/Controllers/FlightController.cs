using EPAY.AIRWAY.KIOSK.API.Controllers.Config;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;
using FluentValidation;
using Microsoft.AspNetCore.Http.Timeouts;
using SearchRequest = EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request.SearchRequest;

namespace EPAY.AIRWAY.KIOSK.API.Controllers;

[Route("api/v1/flight")]
[ApiController]
[Authorize]
public sealed class FlightController(IFlightService flightService) : ParentController
{
    #region Action

    [Authorize(Policy = MyPolicy.Device)]
    [HttpPost("find")]
    [RequestTimeout(CustomTimeoutProfile.Over3M)]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<SearchResponse>), 200)]
    [SwaggerOperation(summary: "Lấy ra thông tin chuyến bay")]
    public async Task<IActionResult> SearchAsync([FromBody] SearchRequest request, [FromServices] IValidator<SearchRequest> validator, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var result = await flightService.SearchAsync(request, cancellationToken);
        return GetBaseResult(200, result);
    }

    [Authorize(Policy = MyPolicy.Device)]
    [HttpGet("master-data")]
    [RequestTimeout(CustomTimeoutProfile.Over15S)]
    [ResponseCache(CacheProfileName = CustomCacheProfile.Any5M)]
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

        return GetBaseResult(200, result);
    }

    [Authorize(Policy = MyPolicy.Device)]
    [HttpPost("additional-services")]
    [RequestTimeout(CustomTimeoutProfile.Over1M)]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<AdditionalServicesResponse>), 200)]
    [SwaggerOperation(summary: "Lấy ra thông tin hành lí, dịch vụ mua thêm")]
    public async Task<IActionResult> AdditionalServicesAsync([FromBody] AdditionalServicesRequest request, [FromServices] IValidator<AdditionalServicesRequest> validator, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var result = await flightService.GetAdditionalServicesAsync(request, cancellationToken);
        return GetBaseResult(200, result);
    }

    [Authorize(Policy = MyPolicy.Device)]
    [HttpPost("verify")]
    [RequestTimeout(CustomTimeoutProfile.Over1M)]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<VerifyResponse>), 200)]
    [SwaggerOperation(summary: "Kiểm tra thông tin chuyến bay")]
    public async Task<IActionResult> VerifyAsync([FromBody] VerifyRequest request, [FromServices] IValidator<VerifyRequest> validator, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var result = await flightService.VerifyAsync(request, cancellationToken);
        return GetBaseResult(200, result);
    }

    [Authorize(Policy = MyPolicy.Device)]
    [HttpPost("booking")]
    [RequestTimeout(CustomTimeoutProfile.Over1M)]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<BookingResponse>), 200)]
    [SwaggerOperation(summary: "Lưu thông tin hành khách và đặt chỗ")]
    public async Task<IActionResult> BookingAsync([FromBody] BookingRequest request, [FromServices] IValidator<BookingRequest> validator, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var result = await flightService.BookingAsync(request, cancellationToken);
        return GetBaseResult(200, result);
    }

    #endregion
}