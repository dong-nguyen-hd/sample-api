using EPAY.AIRWAY.KIOSK.API.Resources.Enums;

namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Authentication.Request;

public sealed class LoginRequest
{
    public string UserName { get; set; }

    [SensitiveData]
    public string Password { get; set; }

    public PlatformType? Type { get; set; }
}
