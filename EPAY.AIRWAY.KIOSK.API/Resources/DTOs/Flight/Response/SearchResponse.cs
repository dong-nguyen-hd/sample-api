namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;

public sealed class SearchResponse
{
    public string? FlightType { get; set; }
    public string? Session { get; set; }
    public List<FareDataResponse>? ListFareData { get; set; }
}

public sealed class FareDataResponse
{
    public int? FareDataId { get; set; }
    public string? Airline { get; set; }

    public int? Adt { get; set; }
    public int? Chd { get; set; }
    public int? Inf { get; set; }

    public int? FareAdt { get; set; }
    public int? FareChd { get; set; }
    public int? FareInf { get; set; }

    public int? TaxAdt { get; set; }
    public int? TaxChd { get; set; }
    public int? TaxInf { get; set; }

    public int? FeeAdt { get; set; }
    public int? FeeChd { get; set; }
    public int? FeeInf { get; set; }

    public int? VatAdt { get; set; }
    public int? VatChd { get; set; }
    public int? VatInf { get; set; }

    public int? ServiceFeeAdt { get; set; }
    public int? ServiceFeeChd { get; set; }
    public int? ServiceFeeInf { get; set; }

    public int? DiscountAdt { get; set; }
    public int? DiscountChd { get; set; }
    public int? DiscountInf { get; set; }

    public int? TotalNetPrice { get; set; }
    public int? TotalServiceFee { get; set; }
    public int? TotalDiscount { get; set; }
    public int? TotalCommission { get; set; }
    public int? TotalPrice { get; set; }

    public List<FlightResponse>? ListFlight { get; set; }
}

public sealed class FlightResponse
{
    public int? FlightId { get; set; }
    public string? Airline { get; set; }
    public string? Operating { get; set; }
    public string? StartPoint { get; set; }
    public string? EndPoint { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? Duration { get; set; }
    public string? FlightNumber { get; set; }
    public int? StopNum { get; set; }
    public string? FlightValue { get; set; }
    public bool? HasDownStop { get; set; }
    public bool? NoRefund { get; set; }
    public string? GroupClass { get; set; }

    public string? FareClass { get; set; }
    public List<FareRulesResponse>? ListFareRules { get; set; }
    public List<SegmentResponse>? ListSegment { get; set; }
}

public sealed class SegmentResponse
{
    public int? Id { get; set; }
    public string? Airline { get; set; }
    public string? StartPoint { get; set; }
    public string? EndPoint { get; set; }
    public DateTime? StartTime { get; set; }
    public string? StartTimeZoneOffset { get; set; }
    public DateTime? EndTime { get; set; }
    public string? EndTimeZoneOffset { get; set; }
    public int? Duration { get; set; }
    public string? FlightNumber { get; set; }
    public AircraftResponse? Plane { get; set; }
    public int? Seat { get; set; }
    public string? Class { get; set; }
    public string? HandBaggage { get; set; }
    public string? AllowanceBaggage { get; set; }
}