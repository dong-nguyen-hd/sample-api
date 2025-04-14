namespace AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;

public sealed class AdditionalServicesResponse
{
    public List<AdditionalServicesInnerResponse>? AdditionalServices { get; set; }
}

public sealed class AdditionalServicesInnerResponse
{
    public string? StartPoint { get; set; }
    public string? EndPoint { get; set; }
    public List<BaggageResponse>? ListBaggage { get; set; }
    public List<AncillaryResponse>? ListAncillary { get; set; }
}

public sealed class BaggageResponse
{
    public string? Airline { get; set; }
    public int? Leg { get; set; }
    public string? Route { get; set; }
    public string? Code { get; set; }
    public string? Currency { get; set; }
    public string? Name { get; set; }
    public long? Price { get; set; }
    public string? Value { get; set; }
    
    public string? Session { get; set; }

    public string? Type { get; set; }

    public string? Description { get; set; }

    public string? StartPoint { get; set; }

    public string? EndPoint { get; set; }

    public string? StatusCode { get; set; }

    public bool? Confirmed { get; set; }
}

public sealed class AncillaryResponse
{
    public string? Airline { get; set; }
    public int? Leg { get; set; }
    public string? Route { get; set; }
    public string? Code { get; set; }
    public string? Currency { get; set; }
    public string? Name { get; set; }
    public long? Price { get; set; }
    public string? Value { get; set; }
    
    public string? Session { get; set; }

    public string? Type { get; set; }

    public string? Description { get; set; }

    public string? StartPoint { get; set; }

    public string? EndPoint { get; set; }

    public string? StatusCode { get; set; }

    public bool? Confirmed { get; set; }
}