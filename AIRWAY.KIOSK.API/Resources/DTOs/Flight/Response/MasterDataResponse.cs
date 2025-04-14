namespace AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;

public sealed class MasterDataResponse
{
    [JsonPropertyName("aircrafts")]
    public List<AircraftResponse>? Aircrafts { get; set; }
    
    [JsonPropertyName("airlines")]
    public List<AirlineResponse>? Airlines { get; set; }
    
    [JsonPropertyName("airports")]
    public List<AirportResponse>? Airports { get; set; }
    
    [JsonPropertyName("popularity")]
    public List<AirportResponse>? Popularity { get; set; }
}