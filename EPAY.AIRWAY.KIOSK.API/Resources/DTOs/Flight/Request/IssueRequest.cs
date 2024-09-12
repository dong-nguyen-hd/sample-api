namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request;

public sealed class IssueRequest
{
    /// <summary>
    /// Mã giao dịch trao đổi giữa Abtrip và Epay
    /// </summary>
    public string? AbTripOrderId { get; set; }
}