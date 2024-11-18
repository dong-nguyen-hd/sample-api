namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;

public sealed class IssueResponse
{
    /// <summary>
    /// Xác định tất cả booking-code đều xuất thành công <br/>
    /// Trường hợp = true: ghi nhận tất cả booking-code ghi nhận trong hệ thống là thành công <br/>
    /// Trường hợp = false: cần kiểm tra tới từng booking-code trong "IssueStatus" <br/>
    /// </summary>
    public bool AllSuccessful { get; set; }
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