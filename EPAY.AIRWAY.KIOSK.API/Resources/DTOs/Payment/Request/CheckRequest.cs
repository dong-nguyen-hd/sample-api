namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Payment.Request;

public sealed class CheckRequest
{
    public string? OrderCode { get; set; }
    public string? BillId { get; set; }
}