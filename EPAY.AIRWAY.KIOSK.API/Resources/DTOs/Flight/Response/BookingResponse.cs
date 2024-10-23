namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;

public sealed class BookingResponse
{
    public string? BillId { get; set; }
    public bool? IsThirdParty { get; set; }
    
    /// <summary>
    /// Mã giao dịch trao đổi giữa Abtrip và Epay
    /// </summary>
    public string? AbTripOrderId { get; set; }
    
    public DateTime? OrderDatetimeUtc { get; set; }
    
    public DateTime? ExpiryDate { get; set; }
    public int? TotalPrice { get; set; }
    public InvoiceResponse? Invoice { get; set; }
    public ContactResponse? Contact { get; set; }
    public List<PassengerResponse>? ListPassenger { get; set; }
    public List<BookingInnerResponse>? ListFareData { get; set; }
}

public sealed class BookingInnerResponse
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