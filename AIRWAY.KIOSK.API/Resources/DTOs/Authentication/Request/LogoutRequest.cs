namespace AIRWAY.KIOSK.API.Resources.DTOs.Authentication.Request;

public sealed class LogoutRequest
{
    [SensitiveData]
    public string? RefreshToken { get; set; }
}
