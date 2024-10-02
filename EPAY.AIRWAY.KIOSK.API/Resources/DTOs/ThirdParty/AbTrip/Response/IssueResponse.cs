namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip.Response;

public sealed class IssueResponse : BaseResponse
{
    [JsonPropertyName("Data")]
    public Dictionary<string, IssueInfoResponse?>? Data { get; set; }
}

public class IssueInfoInnerResponse
{
    [JsonPropertyName("BookingCode")]
    public string? BookingCode { get; set; }
    
    [JsonPropertyName("TicketNumber")]
    public string? TicketNumber { get; set; }
    
    [JsonPropertyName("PassengerType")]
    public string? PassengerType { get; set; }
    
    [JsonPropertyName("TotalPrice")]
    public int? TotalPrice { get; set; }
    
    [JsonPropertyName("IssueDateTime")]
    public DateTime? IssueDatetime { get; set; }
}

public class IssueInfoResponse
{
    [JsonPropertyName("ListTicket")]
    public List<IssueInfoInnerResponse>? ListTicket { get; set; }
    
    [JsonPropertyName("Status")]
    public bool? Status { get; set; }
    
    [JsonPropertyName("ErrorCode")]
    public string? ErrorCode { get; set; }
}