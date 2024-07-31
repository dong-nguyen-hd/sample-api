namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.PaymentGateway.Response;

public class DecryptResponse
{
    [JsonPropertyName("transId")]
    public string? TransId { get; set; }

    [JsonPropertyName("merchantCode")]
    public string? MerchantCode { get; set; }

    [JsonPropertyName("timeResponse")]
    public long? TimeResponse { get; set; }

    [JsonPropertyName("errorCode")]
    public int? ErrorCode { get; set; }

    [JsonPropertyName("errorDesc")]
    public string? ErrorDesc { get; set; }

    [JsonPropertyName("messageType")]
    public string? MessageType { get; set; }
}