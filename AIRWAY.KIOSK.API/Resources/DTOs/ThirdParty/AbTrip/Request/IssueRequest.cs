namespace AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip.Request;

public sealed class IssueRequest : BaseRequest
{
    [JsonPropertyName("OrderID")]
    public string? OrderId { get; set; }
}