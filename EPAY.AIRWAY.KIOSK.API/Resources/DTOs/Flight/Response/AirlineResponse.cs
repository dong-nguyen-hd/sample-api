namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;

public sealed class AirlineResponse
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("logo")]
    public string? Logo { get; set; }
}