using EPAY.AIRWAY.KIOSK.API.Controllers.Config;
using EPAY.AIRWAY.KIOSK.API.Domain.Context.Config;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Account.Response;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Authentication.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Authentication.Response;

namespace EPAY.AIRWAY.KIOSK.API.Controllers;

[Route("api/v1/authentication")]
[ApiController]
public sealed class AuthenticationController(ITokenManagementService tokenManagementService, IMapper mapper) : ParentController(mapper)
{
    #region Action

    [AllowAnonymous]
    [HttpPost("login")]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<AccessTokenResponse>), 200)]
    [SwaggerOperation(summary: "Đăng nhập")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        string userAgent = Request.Headers["User-Agent"].ToString();
        var result = await tokenManagementService.GenerateTokensAsync(loginRequest, DateTime.UtcNow, userAgent, cancellationToken);

        return result.CodeMessage == CodeMessage._0000 ? Ok(result) : Unauthorized(result);
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<TokenResponse>), 200)]
    [SwaggerOperation(summary: "Sử dụng refresh-token tạo mới access-token")]
    public async Task<IActionResult> GenerateNewTokensAsync([FromBody] RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken)
    {
        refreshTokenRequest.UserAgent = Request.Headers["User-Agent"].ToString();
        var result = await tokenManagementService.GenerateNewTokensAsync(refreshTokenRequest, DateTime.UtcNow, cancellationToken);

        return result.CodeMessage == CodeMessage._0000 ? Ok(result) : Unauthorized(result);
    }

    [Authorize]
    [HttpPost("logout")]
    [ResponseCache(CacheProfileName = CustomCacheProfile.NoCache)]
    [ProducesResponseType(typeof(BaseResult<bool>), 200)]
    [SwaggerOperation(summary: "Đăng xuất")]
    public async Task<IActionResult> LogoutAsync([FromBody] LogoutRequest logoutRequest, CancellationToken cancellationToken)
    {
        var result = await tokenManagementService.LogoutAsync(logoutRequest, cancellationToken);

        return Ok(result);
    }

    #endregion
}