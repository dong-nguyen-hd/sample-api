namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request;

public sealed class AdditionalServicesRequest
{
    public List<FareDataRequest>? ListFareData { get; set; }
}

public sealed class FareDataRequest
{
    public string? Session { get; set; }
    public int? FareDataId { get; set; }
    public List<FlightRequest>? ListFlight { get; set; }
}

public sealed class FlightRequest
{
    public string? FlightValue { get; set; }
    public string? StartPoint { get; set; }
    public string? EndPoint { get; set; }
}