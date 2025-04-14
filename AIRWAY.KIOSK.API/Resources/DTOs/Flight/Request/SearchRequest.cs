namespace AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request;

public sealed class SearchRequest
{
    public int? Adt { get; set; }
    public int? Chd { get; set; }
    public int? Inf { get; set; }
    public List<SearchFlightRequest>? ListFlight { get; set; }
}

public sealed class SearchFlightRequest
{
    public string? Airline { get; set; }
    public string? StartPoint { get; set; }
    public string? EndPoint { get; set; }
    public DateTime? DepartDate { get; set; }
}