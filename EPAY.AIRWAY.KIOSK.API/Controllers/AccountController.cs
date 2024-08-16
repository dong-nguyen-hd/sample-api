using EPAY.AIRWAY.KIOSK.API.Controllers.Config;
using EPAY.AIRWAY.KIOSK.API.Domain.Context.Config;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Account.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Account.Response;
using FluentValidation;

namespace EPAY.AIRWAY.KIOSK.API.Controllers;

[Route("api/v1/account")]
[ApiController]
[Authorize]
public sealed class AccountController(IAccountService accountService, IMapper mapper) : ParentController(mapper)
{
    #region Action

    [Authorize(Policy = MyPolicy.Administrator)]
    [HttpPost("create")]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<AccountResponse>), 200)]
    [SwaggerOperation(summary: "Tạo mới một tài khoản")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateRequest request, [FromServices] IValidator<CreateRequest> validator, CancellationToken cancellationToken)
    {
        var validate = await validator.ValidateAsync(request, cancellationToken);
        if (!validate.IsValid)
            return ProduceErrorResponse(validate, this.ModelState);

        var result = await accountService.CreateAsync(request, cancellationToken);
        return Ok(result);
    }

    [Authorize(Policy = MyPolicy.Administrator)]
    [HttpGet("get-roles")]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<string[]>), 200)]
    [SwaggerOperation(summary: "Lấy role có trong hệ thống")]
    public IActionResult GetRoles()
    {
        string[] roles =
        [
            MyPolicy.Editor,
            MyPolicy.Device,
            MyPolicy.Viewer,
        ];

        return Ok(GetBaseResult(CodeMessage._0000, data: roles));
    }

    [Authorize(Policy = MyPolicy.Administrator)]
    [HttpPost("change-password/{id}")]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<AccountResponse>), 200)]
    [SwaggerOperation(summary: "Thay đổi mật khẩu")]
    public async Task<IActionResult> UpdatePasswordAsync([FromRoute] string id, [FromBody] UpdatePasswordAccountRequest request, CancellationToken cancellationToken)
    {
        // Check if the id belongs to me
        var identifier = (User.Identity as ClaimsIdentity).FindFirst(ClaimTypes.NameIdentifier).Value;
        if (identifier != id)
            return Ok(GetBaseResult<AccountResponse>(CodeMessage._100));

        // Checking duplicate password
        if (request.OldPassword == request.NewPassword)
            return Ok(GetBaseResult<AccountResponse>(CodeMessage._100));

        var result = await accountService.UpdatePasswordAsync(id, request, cancellationToken);

        return Ok(result);
    }

    [Authorize(Policy = MyPolicy.Administrator)]
    [HttpPost("delete/{id}")]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<AccountResponse>), 200)]
    [SwaggerOperation(summary: "Xoá một tài khoản")]
    public async Task<IActionResult> DeleteAsync([FromRoute] string id, CancellationToken cancellationToken)
    {
        if (id == AccountConfig.AdminId || id == AccountConfig.DeviceId)
            return Ok(GetBaseResult<AccountResponse>(CodeMessage._100));

        var result = await accountService.DeleteAsync(id, cancellationToken);

        return Ok(result);
    }

    [Authorize(Policy = MyPolicy.Administrator)]
    [HttpPost("update/{id}")]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<AccountResponse>), 200)]
    [SwaggerOperation(summary: "Cập nhật thông tin về tài khoản")]
    public async Task<IActionResult> UpdateAsync([FromRoute] string id, [FromBody] UpdateRequest request, CancellationToken cancellationToken)
    {
        // Check if the id belongs to me
        var identifier = (User.Identity as ClaimsIdentity).FindFirst(ClaimTypes.NameIdentifier).Value;
        if (identifier != AccountConfig.AdminId && identifier != id)
            return Ok(GetBaseResult<AccountResponse>(CodeMessage._100));

        var result = await accountService.UpdateAsync(id, request, cancellationToken);

        return Ok(result);
    }

    #endregion
}