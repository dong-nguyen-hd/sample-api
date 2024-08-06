namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;

public sealed class MasterDataResponse
{
    public List<AircraftsResponse>? Aircrafts { get; set; }
    public List<AirlinesResponse>? Airlines { get; set; }
    public List<AirportsResponse>? Airports { get; set; }
}