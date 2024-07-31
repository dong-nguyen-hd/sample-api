namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.PaymentGateway.Request;

public sealed class CheckOrderRequest : DecryptRequest
{
    /// <summary>
    /// Mã đơn hàng do Merchant tạo
    /// </summary>
    [JsonPropertyName("orderCode")]
    public string? OrderCode { get; set; }
}
