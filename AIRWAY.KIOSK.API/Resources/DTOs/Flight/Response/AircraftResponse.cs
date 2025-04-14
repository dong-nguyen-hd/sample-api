namespace AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;

public sealed class AircraftResponse
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }
    
    [JsonPropertyName("manufacturer")]
    public string? Manufacturer { get; set; }
    
    [JsonPropertyName("model")]
    public string? Model { get; set; }
}