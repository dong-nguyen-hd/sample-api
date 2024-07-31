namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.PaymentGateway.Request;

public sealed class BaseRequest<T>
{
    [JsonPropertyName("merchantCode")]
    public string? MerchantCode { get; set; }

    [JsonPropertyName("data")]
    public T? Data { get; set; }

    [JsonPropertyName("signature")]
    public string? Signature { get; set; }
}
