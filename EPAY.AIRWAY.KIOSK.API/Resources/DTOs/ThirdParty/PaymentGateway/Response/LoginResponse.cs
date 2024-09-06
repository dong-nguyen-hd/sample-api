namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.PaymentGateway.Response;

public sealed class LoginResponse : DecryptResponse
{
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    [JsonPropertyName("expiresIn")]
    public int? ExpiresIn { get; set; }
}