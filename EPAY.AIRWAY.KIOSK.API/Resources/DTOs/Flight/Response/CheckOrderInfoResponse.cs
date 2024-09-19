namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;

public sealed class CheckOrderInfoResponse
{
    public string? BillId { get; set; }
    public bool? IsPaylater { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int? TotalPrice { get; set; }
    public InvoiceResponse? Invoice { get; set; }
    public ContactResponse? Contact { get; set; }
    public List<PassengerResponse>? ListPassenger { get; set; }
    public List<CheckOrderInfoInnerResponse>? ListFareData { get; set; }
}

public sealed class CheckOrderInfoInnerResponse
{
    public AirportResponse? StartPoint { get; set; }
    public AirportResponse? EndPoint { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public int? FareDataId { get; set; }

    public int? Adt { get; set; }
    public int? Chd { get; set; }
    public int? Inf { get; set; }

    public int? UnitPriceAdt { get; set; }
    public int? UnitPriceChd { get; set; }
    public int? UnitPriceInf { get; set; }

    public int? TotalPrice { get; set; }

    public string? FlightNumber { get; set; }

    public AirlineResponse? Airline { get; set; }

    public AirlineResponse? Operating { get; set; }
}