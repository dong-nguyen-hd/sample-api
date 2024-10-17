using EPAY.AIRWAY.KIOSK.API.Controllers.Config;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Device.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Device.Response;
using FluentValidation;
using Microsoft.AspNetCore.Http.Timeouts;

namespace EPAY.AIRWAY.KIOSK.API.Controllers;

[Route("api/v1/device")]
[ApiController]
[Authorize(Policy = MyPolicy.Administrator)]
public sealed class DeviceController(IDeviceService deviceService) : ParentController
{
    #region Action

    [Authorize(Policy = MyPolicy.Administrator)]
    [HttpPost("create")]
    [RequestTimeout(CustomTimeoutProfile.Over15S)]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<DeviceResponse>), 200)]
    [SwaggerOperation(summary: "Tạo mới một thiết bị")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateRequest request, [FromServices] IValidator<CreateRequest> validator, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var result = await deviceService.CreateAsync(request, cancellationToken);
        return GetBaseResult(200, result);
    }

    [Authorize(Policy = MyPolicy.Administrator)]
    [HttpPost("update/{id}")]
    [RequestTimeout(CustomTimeoutProfile.Over15S)]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<DeviceResponse>), 200)]
    [SwaggerOperation(summary: "Cập nhật thông tin thiết bị")]
    public async Task<IActionResult> GetByCodeAsync([FromRoute] Guid id, [FromBody] UpdateRequest request, [FromServices] IValidator<UpdateRequest> validator, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var result = await deviceService.UpdateAsync(id, request, cancellationToken);
        return GetBaseResult(200, result);
    }

    [Authorize(Policy = MyPolicy.Administrator)]
    [HttpGet("get-by-code/{id}")]
    [RequestTimeout(CustomTimeoutProfile.Over15S)]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<DeviceResponse>), 200)]
    [SwaggerOperation(summary: "Lấy thông tin thiết bị dựa vào mã id")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await deviceService.GetByIdAsync(id, cancellationToken);
        return GetBaseResult(200, result);
    }

    #endregion
}