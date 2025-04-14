namespace AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;

public sealed class AirportResponse
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("cityName")]
    public string? CityName { get; set; }
    
    [JsonPropertyName("countryCode")]
    public string? CountryCode { get; set; }
    
    [JsonPropertyName("countryName")]
    public string? CountryName { get; set; }
}