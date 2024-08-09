namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;

public sealed class MasterDataResponse
{
    [JsonPropertyName("aircrafts")]
    public List<AircraftsResponse>? Aircrafts { get; set; }
    
    [JsonPropertyName("airlines")]
    public List<AirlinesResponse>? Airlines { get; set; }
    
    [JsonPropertyName("airports")]
    public List<AirportsResponse>? Airports { get; set; }
}