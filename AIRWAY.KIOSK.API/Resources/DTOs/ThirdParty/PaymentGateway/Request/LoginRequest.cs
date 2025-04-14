namespace AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.PaymentGateway.Request;

public sealed class LoginRequest : DecryptRequest
{
    [JsonPropertyName("userName")]
    public string? UserName { get; set; }

    [JsonPropertyName("password")]
    public string? Password { get; set; }
}
