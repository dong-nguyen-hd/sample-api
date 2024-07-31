namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.PaymentGateway.Request;

public class DecryptRequest
{
    [JsonPropertyName("merchantCode")]
    public string? MerchantCode { get; set; }

    /// <summary>
    /// 04.03.24: Trao đổi với cổng thanh toán, bảo không truyền cũng được :D
    /// </summary>
    [JsonPropertyName("transId")]
    public string? TransId { get; set; }

    [JsonPropertyName("timeRequest")]
    public long? TimeRequest { get; set; }

    [JsonPropertyName("messageType")]
    public string? MessageType { get; set; }
}
