namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;

public sealed class BookingResponse
{
    public bool IsSuccess { get; set; }
    public string? BillId { get; set; }
}