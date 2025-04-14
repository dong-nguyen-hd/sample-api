using AIRWAY.KIOSK.API.Controllers.Config;
using AIRWAY.KIOSK.API.Domain.Context.Config;
using AIRWAY.KIOSK.API.Domain.Services;
using AIRWAY.KIOSK.API.Resources.DTOs.Account.Request;
using AIRWAY.KIOSK.API.Resources.DTOs.Account.Response;
using FluentValidation;
using Microsoft.AspNetCore.Http.Timeouts;

namespace AIRWAY.KIOSK.API.Controllers;

[Route("api/v1/account")]
[ApiController]
[Authorize(Policy = MyPolicy.Administrator)]
public sealed class AccountController(IAccountService accountService) : ParentController
{
    #region Action

    [Authorize(Policy = MyPolicy.Administrator)]
    [HttpPost("create")]
    [RequestTimeout(CustomTimeoutProfile.Over15S)]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<AccountResponse>), 200)]
    [SwaggerOperation(summary: "Tạo mới một tài khoản")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateRequest request, [FromServices] IValidator<CreateRequest> validator, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var result = await accountService.CreateAsync(request, cancellationToken);
        return GetBaseResult(200, result);
    }

    [Authorize(Policy = MyPolicy.Administrator)]
    [HttpGet("get-roles")]
    [RequestTimeout(CustomTimeoutProfile.Over15S)]
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

        return GetBaseResult(200, GetBaseResult(CodeMessage._0000, data: roles));
    }

    [Authorize(Policy = MyPolicy.Administrator)]
    [HttpPost("change-password/{id}")]
    [RequestTimeout(CustomTimeoutProfile.Over15S)]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<AccountResponse>), 200)]
    [SwaggerOperation(summary: "Thay đổi mật khẩu")]
    public async Task<IActionResult> UpdatePasswordAsync([FromRoute] string id, [FromBody] UpdatePasswordAccountRequest request, CancellationToken cancellationToken)
    {
        // Checking duplicate password
        if (request.OldPassword == request.NewPassword)
            return Ok(GetBaseResult<AccountResponse>(CodeMessage._3005));

        var result = await accountService.UpdatePasswordAsync(id, request, cancellationToken);

        return GetBaseResult(200, result);
    }

    [Authorize(Policy = MyPolicy.Administrator)]
    [HttpPost("delete/{id}")]
    [RequestTimeout(CustomTimeoutProfile.Over15S)]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<AccountResponse>), 200)]
    [SwaggerOperation(summary: "Xoá một tài khoản")]
    public async Task<IActionResult> DeleteAsync([FromRoute] string id, CancellationToken cancellationToken)
    {
        if (id == AccountConfig.AdminId)
            return Ok(GetBaseResult<AccountResponse>(CodeMessage._3005));

        var result = await accountService.DeleteAsync(id, cancellationToken);

        return GetBaseResult(200, result);
    }

    [Authorize(Policy = MyPolicy.Administrator)]
    [HttpPost("update/{id}")]
    [RequestTimeout(CustomTimeoutProfile.Over15S)]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<AccountResponse>), 200)]
    [SwaggerOperation(summary: "Cập nhật thông tin về tài khoản")]
    public async Task<IActionResult> UpdateAsync([FromRoute] string id, [FromBody] UpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await accountService.UpdateAsync(id, request, cancellationToken);

        return GetBaseResult(200, result);
    }

    #endregion
}