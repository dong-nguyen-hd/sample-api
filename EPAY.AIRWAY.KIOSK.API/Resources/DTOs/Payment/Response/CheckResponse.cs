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
    public string? AbTripOrderId { get; set; }
    public bool IsSuccess { get; set; }
    public DateTime? PaidDatetimeUtc { get; set; }
    public DateTime? ExpiredDatetimeUtc { get; set; }
    public CheckServiceResponse? Service { get; set; }
}

public sealed class CheckServiceResponse
{
    public ContactResponse? Contact { get; set; }
    public MyEnum.TicketType TicketType { get; set; }
    public int TotalTicket { get; set; }
    public PointResponse? PointOne { get; set; }
    public PointResponse? PointTwo { get; set; }
}

public sealed class PointResponse
{
    public string? BookingCode { get; set; }
    
    public AirlineResponse? Airline { get; set; }

    public AirportResponse? StartPoint { get; set; }
    public DateTime? StartDate { get; set; }

    public AirportResponse? EndPoint { get; set; }
    public DateTime? EndDate { get; set; }
}