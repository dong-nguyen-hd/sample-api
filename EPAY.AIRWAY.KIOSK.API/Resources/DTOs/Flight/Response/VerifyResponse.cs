namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;

public sealed class VerifyResponse
{
    public int TotalPrice { get; set; }
    public List<FareDataResponse>? ListFareData { get; set; }
}

public sealed class FareDataResponse
{
    public string? Session { get; set; }
    public int? FareDataId { get; set; }
    public List<FlightResponse>? ListFlight { get; set; }
}

public sealed class FlightResponse
{
    public string? FlightValue { get; set; }
    public string? StartPoint { get; set; }
    public string? EndPoint { get; set; }
}