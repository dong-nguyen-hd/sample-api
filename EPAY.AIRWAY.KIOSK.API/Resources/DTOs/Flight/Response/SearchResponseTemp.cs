namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;

public sealed class SearchResponseTemp
{
    public MyEnum.FlightType FlightType { get; set; }
    public string? Session { get; set; }
    public int? Itinerary { get; set; }
    public List<SearchDetailResponse>? SearchDetail { get; set; }
}

public sealed class SearchDetailResponse
{
    public AirportsResponse? StartPoint { get; set; }
    public AirportsResponse? EndPoint { get; set; }
    public List<FilghtDetailResponse>? ListFlight { get; set; }
}

public sealed class FilghtDetailResponse
{
    public FilghtInnerResponse? FlightStart { get; set; }
    public FilghtInnerResponse? FlightEnd { get; set; }
}

public sealed class FilghtInnerResponse
{
    public int? Index { get; set; }
    public string? FlightNumber { get; set; }
    public string? FlightValue { get; set; }
    public AirlinesResponse? Airline { get; set; }
    public AirlinesResponse? Operating { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? Duration { get; set; }
    public int? StopNum { get; set; }
    public bool? HasUpgradeClass { get; set; }
    public List<FareResponse>? ListFareClass { get; set; }
}

public sealed class FareResponse
{
    public int? FareDataId { get; set; }
    public int? Adt { get; set; }
    public int? Chd { get; set; }
    public int? Inf { get; set; }
    public int? UnitPriceAdt { get; set; }
    public int? UnitPriceChd { get; set; }
    public int? UnitPriceInf { get; set; }
    public int? TotalPrice { get; set; }
    public string? GroupClass { get; set; }
    public string? FareClass { get; set; }
    public FareRulesResponse? FareRules { get; set; }
    public List<FlightSegmentResponse>? ListSegment { get; set; }
}

public sealed class FlightSegmentResponse
{
    public string? FlightNumber { get; set; }
    public AirlinesResponse? Airline { get; set; }
    public AirlinesResponse? Operating { get; set; }
    public AirportsResponse? StartPoint { get; set; }
    public AirportsResponse? EndPoint { get; set; }
    public DateTime? StartTime { get; set; }
    public string? StartTimeZoneOffset { get; set; }
    public DateTime? EndTime { get; set; }
    public string? EndTimeZoneOffset { get; set; }
    public int? Duration { get; set; }
    public AircraftsResponse? Plane { get; set; }
    public int? Seat { get; set; }
    public string? Class { get; set; }
    public string? HandBaggage { get; set; }
    public string? AllowanceBaggage { get; set; }
}