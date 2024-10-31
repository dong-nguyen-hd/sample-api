using EPAY.AIRWAY.KIOSK.API.Controllers.Config;
using EPAY.AIRWAY.KIOSK.API.Domain.Context.Config;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Account.Response;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Authentication.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Authentication.Response;
using FluentValidation;
using Microsoft.AspNetCore.Http.Timeouts;

namespace EPAY.AIRWAY.KIOSK.API.Controllers;

[Route("api/v1/authentication")]
[ApiController]
public sealed class AuthenticationController(ITokenManagementService tokenManagementService) : ParentController()
{
    #region Action

    [AllowAnonymous]
    [HttpPost("login")]
    [RequestTimeout(CustomTimeoutProfile.Over15S)]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<AccessTokenResponse>), 200)]
    [SwaggerOperation(summary: "Đăng nhập")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest loginRequest, [FromServices] IValidator<LoginRequest> validator, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(loginRequest, cancellationToken);
        
        string? userAgent = Request.Headers.UserAgent;
        var result = await tokenManagementService.GenerateTokensAsync(loginRequest, DateTime.UtcNow, userAgent, cancellationToken);

        return result.CodeMessage == CodeMessage._0000 ? GetBaseResult(200, result) : GetBaseResult(401, result);
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    [RequestTimeout(CustomTimeoutProfile.Over15S)]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<TokenResponse>), 200)]
    [SwaggerOperation(summary: "Sử dụng refresh-token tạo mới access-token")]
    public async Task<IActionResult> GenerateNewTokensAsync([FromBody] RefreshTokenRequest refreshTokenRequest, [FromServices] IValidator<RefreshTokenRequest> validator, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(refreshTokenRequest, cancellationToken);
        
        refreshTokenRequest.UserAgent = Request.Headers["User-Agent"].ToString();
        var result = await tokenManagementService.GenerateNewTokensAsync(refreshTokenRequest, DateTime.UtcNow, cancellationToken);

        return result.CodeMessage == CodeMessage._0000 ? GetBaseResult(200, result) : GetBaseResult(401, result);
    }

    [Authorize]
    [HttpPost("logout")]
    [RequestTimeout(CustomTimeoutProfile.Over15S)]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<bool>), 200)]
    [SwaggerOperation(summary: "Đăng xuất")]
    public async Task<IActionResult> LogoutAsync([FromBody] LogoutRequest logoutRequest, [FromServices] IValidator<LogoutRequest> validator, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(logoutRequest, cancellationToken);
        
        var result = await tokenManagementService.LogoutAsync(logoutRequest, cancellationToken);

        return GetBaseResult(200, result);
    }

    #endregion
}