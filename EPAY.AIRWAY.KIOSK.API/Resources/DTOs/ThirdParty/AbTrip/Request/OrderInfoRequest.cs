namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip.Request;

public sealed class OrderInfoRequest : BaseRequest
{
    [JsonPropertyName("OrderID")]
    public string? OrderId { get; set; }
}