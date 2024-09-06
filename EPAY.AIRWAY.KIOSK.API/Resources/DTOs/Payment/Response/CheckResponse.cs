using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;
using EPAY.AIRWAY.KIOSK.API.Resources.Enums;

namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Payment.Response;

public sealed class CheckResponse
{
    public int TotalAmount { get; set; }
    public PaymentType PaymentType { get; set; }
    public PlatformType PlatformType { get; set; }
    public string? OrderCode { get; set; }
    public string? BillId { get; set; }
    public string? AbTripCode { get; set; }
    public bool IsSuccess { get; set; }
    public DateTime? PaidDatetimeUtc { get; set; }
    public DateTime? ExpiredDatetimeUtc { get; set; }
    public CheckServiceResponse? Service { get; set; }
}

public sealed class CheckServiceResponse
{
    public ContactResponse? Contact { get; set; }
    public PointResponse? StartPoint { get; set; }
    public PointResponse? EndPoint { get; set; }
    public MyEnum.FlightType FlightType { get; set; }
    public int TotalTicket { get; set; }
}

public sealed class PointResponse
{
    public AirportsResponse? Airport { get; set; }
    public string? BookingCode { get; set; }
    
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}