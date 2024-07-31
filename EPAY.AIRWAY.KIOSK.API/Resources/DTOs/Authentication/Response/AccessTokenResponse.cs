namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Authentication.Response;

using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Account.Response;

public sealed class AccessTokenResponse : AccountResponse
{
    public TokenResponse? TokenResponse { get; set; }
}

public sealed class TokenResponse
{
    public string Id { get; set; }

    public DateTime UtcNow { get; set; } = DateTime.UtcNow;

    [SensitiveData]
    public string? RefreshToken { get; set; }

    public DateTime RefreshTokenExpireTimeUTC { get; set; }

    [SensitiveData]
    public string? AccessToken { get; set; }

    public DateTime AccessTokenExpireTimeUTC { get; set; }
}
