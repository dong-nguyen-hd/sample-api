namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Account.Request;

public sealed class UpdatePasswordAccountRequest
{
    [SensitiveData]
    public string OldPassword { get; set; }

    [SensitiveData]
    public string NewPassword { get; set; }
}
