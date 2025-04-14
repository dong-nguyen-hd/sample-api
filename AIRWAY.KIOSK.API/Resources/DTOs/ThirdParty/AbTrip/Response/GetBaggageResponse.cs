namespace AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip.Response;

public class GetBaggageResponse : BaseResponse
{
    [JsonPropertyName("ListBaggage")]
    public List<GetAncillaryInnerResponse>? ListBaggage { get; set; }
}

public sealed class GetBaggageInnerResponse
{
    [JsonPropertyName("Session")]
    public string? Session { get; set; }

    [JsonPropertyName("Airline")]
    public string? Airline { get; set; }

    [JsonPropertyName("Code")]
    public string? Code { get; set; }
    
    [JsonPropertyName("Value")]
    public string? Value { get; set; }

    [JsonPropertyName("Type")]
    public string? Type { get; set; }

    [JsonPropertyName("Name")]
    public string? Name { get; set; }

    [JsonPropertyName("Description")]
    public string? Description { get; set; }

    [JsonPropertyName("Price")]
    public long? Price { get; set; }

    [JsonPropertyName("Currency")]
    public string? Currency { get; set; }

    [JsonPropertyName("StartPoint")]
    public string? StartPoint { get; set; }

    [JsonPropertyName("EndPoint")]
    public string? EndPoint { get; set; }

    [JsonPropertyName("Route")]
    public string? Route { get; set; }

    [JsonPropertyName("Leg")]
    public int? Leg { get; set; }

    [JsonPropertyName("StatusCode")]
    public string? StatusCode { get; set; }

    [JsonPropertyName("Confirmed")]
    public bool? Confirmed { get; set; }
}