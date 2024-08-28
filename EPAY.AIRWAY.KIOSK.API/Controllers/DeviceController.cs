using EPAY.AIRWAY.KIOSK.API.Controllers.Config;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Device.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Device.Response;

namespace EPAY.AIRWAY.KIOSK.API.Controllers;

[Route("api/v1/device")]
[ApiController]
[Authorize(Policy = MyPolicy.Administrator)]
public sealed class DeviceController(IDeviceService deviceService) : ParentController
{
    #region Action

    [Authorize(Policy = MyPolicy.Administrator)]
    [HttpPost("create")]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<DeviceResponse>), 200)]
    [SwaggerOperation(summary: "Tạo mới một thiết bị")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateRequest request, CancellationToken cancellationToken)
    {
        var result = await deviceService.CreateAsync(request, cancellationToken);
        return Ok(result);
    }

    [Authorize(Policy = MyPolicy.Administrator)]
    [HttpPost("update")]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<DeviceResponse>), 200)]
    [SwaggerOperation(summary: "Cập nhật thông tin thiết bị")]
    public async Task<IActionResult> GetByCodeAsync(string id, [FromBody] UpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await deviceService.UpdateAsync(id, request, cancellationToken);
        return Ok(result);
    }

    [Authorize(Policy = MyPolicy.Administrator)]
    [HttpGet("get-by-code/{code}")]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<DeviceResponse>), 200)]
    [SwaggerOperation(summary: "Lấy thông tin thiết bị dựa vào mã code")]
    public async Task<IActionResult> GetByCodeAsync([FromRoute] string code, CancellationToken cancellationToken)
    {
        var result = await deviceService.GetByCodeAsync(code, cancellationToken);
        return Ok(result);
    }

    #endregion
}