namespace AIRWAY.KIOSK.API.Resources.DTOs.Authentication.Request;

public sealed class RefreshTokenRequest
{
    //[SensitiveData]
    public string RefreshToken { get; set; }

    [JsonIgnore]
    public string? UserAgent { get; set; }
}
