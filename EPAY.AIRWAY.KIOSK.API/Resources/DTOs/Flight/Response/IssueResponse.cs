namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;

public sealed class IssueResponse
{
    public Dictionary<string, IssueInnerResponse>? IssueStatus { get; set; }
}

public sealed class IssueInnerResponse
{
    public bool TicketIssued { get; set; }
    public List<TicketDetailResponse>? Tickets { get; set; }
}

public sealed class TicketDetailResponse
{
    public string? TicketNumber { get; set; }
    public DateTime? IssueDatetimeUtc { get; set; }
    public MyEnum.PassengerType? PassengerType { get; set; }
    public int? TotalPrice { get; set; }
}