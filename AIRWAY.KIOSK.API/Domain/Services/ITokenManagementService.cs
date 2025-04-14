using AIRWAY.KIOSK.API.Resources.DTOs.Authentication.Request;
using AIRWAY.KIOSK.API.Resources.DTOs.Authentication.Response;

namespace AIRWAY.KIOSK.API.Domain.Services;

public interface ITokenManagementService : IBaseService
{
    /// <summary>
    /// Chức năng: tạo mới một access-token bằng refresh-token
    /// </summary>
    /// <param name="refreshTokenRequest"></param>
    /// <param name="utcNow"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<TokenResponse>> GenerateNewTokensAsync(RefreshTokenRequest refreshTokenRequest, DateTime utcNow, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: đăng xuất
    /// </summary>
    /// <param name="logoutRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<bool>> LogoutAsync(LogoutRequest logoutRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: tạo access-token bằng thông tin đăng nhập
    /// </summary>
    /// <param name="loginRequest"></param>
    /// <param name="utcNow"></param>
    /// <param name="userAgent"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<AccessTokenResponse>> GenerateTokensAsync(LoginRequest loginRequest, DateTime utcNow, string? userAgent, CancellationToken cancellationToken = default);
}