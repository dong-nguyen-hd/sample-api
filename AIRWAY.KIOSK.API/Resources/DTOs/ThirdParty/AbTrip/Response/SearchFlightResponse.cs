namespace AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip.Response;

public sealed class SearchFlightResponse : BaseResponse
{
    [JsonPropertyName("FlightType")]
    public string? FlightType { get; set; }

    [JsonPropertyName("Session")]
    public string? Session { get; set; }

    [JsonPropertyName("Itinerary")]
    public int? Itinerary { get; set; }

    [JsonPropertyName("ListFareData")]
    public List<SearchFlightInner>? ListFareData { get; set; }
}

public sealed class SearchFlightInner
{
    [JsonPropertyName("FareDataId")]
    public int? FareDataId { get; set; }

    [JsonPropertyName("Airline")]
    public string? Airline { get; set; }

    [JsonPropertyName("System")]
    public string? System { get; set; }

    [JsonPropertyName("Itinerary")]
    public int? Itinerary { get; set; }

    [JsonPropertyName("Leg")]
    public int? Leg { get; set; }

    [JsonPropertyName("Promo")]
    public bool? Promo { get; set; }

    [JsonPropertyName("FullFare")]
    public bool? FullFare { get; set; }

    [JsonPropertyName("Currency")]
    public string? Currency { get; set; }

    [JsonPropertyName("Tourcode")]
    public string? Tourcode { get; set; }

    [JsonPropertyName("FareType")]
    public string? FareType { get; set; }

    [JsonPropertyName("CacheAge")]
    public int? CacheAge { get; set; }

    [JsonPropertyName("Availability")]
    public int? Availability { get; set; }

    [JsonPropertyName("Adt")]
    public int? Adt { get; set; }

    [JsonPropertyName("Chd")]
    public int? Chd { get; set; }

    [JsonPropertyName("Inf")]
    public int? Inf { get; set; }

    [JsonPropertyName("FareAdt")]
    public int? FareAdt { get; set; }

    [JsonPropertyName("FareChd")]
    public int? FareChd { get; set; }

    [JsonPropertyName("FareInf")]
    public int? FareInf { get; set; }

    [JsonPropertyName("TaxAdt")]
    public int? TaxAdt { get; set; }

    [JsonPropertyName("TaxChd")]
    public int? TaxChd { get; set; }

    [JsonPropertyName("TaxInf")]
    public int? TaxInf { get; set; }

    [JsonPropertyName("FeeAdt")]
    public int? FeeAdt { get; set; }

    [JsonPropertyName("FeeChd")]
    public int? FeeChd { get; set; }

    [JsonPropertyName("FeeInf")]
    public int? FeeInf { get; set; }

    [JsonPropertyName("VatAdt")]
    public int? VatAdt { get; set; }

    [JsonPropertyName("VatChd")]
    public int? VatChd { get; set; }

    [JsonPropertyName("VatInf")]
    public int? VatInf { get; set; }

    [JsonPropertyName("ServiceFeeAdt")]
    public int? ServiceFeeAdt { get; set; }

    [JsonPropertyName("ServiceFeeChd")]
    public int? ServiceFeeChd { get; set; }

    [JsonPropertyName("ServiceFeeInf")]
    public int? ServiceFeeInf { get; set; }

    [JsonPropertyName("DiscountAdt")]
    public int? DiscountAdt { get; set; }

    [JsonPropertyName("DiscountChd")]
    public int? DiscountChd { get; set; }

    [JsonPropertyName("DiscountInf")]
    public int? DiscountInf { get; set; }

    [JsonPropertyName("TotalNetPrice")]
    public int? TotalNetPrice { get; set; }

    [JsonPropertyName("TotalServiceFee")]
    public int? TotalServiceFee { get; set; }

    [JsonPropertyName("TotalDiscount")]
    public int? TotalDiscount { get; set; }

    [JsonPropertyName("TotalCommission")]
    public int? TotalCommission { get; set; }

    [JsonPropertyName("TotalPrice")]
    public int? TotalPrice { get; set; }

    [JsonPropertyName("ListFlight")]
    public List<FlightResponse>? ListFlight { get; set; }

    [JsonPropertyName("ListXmlRulesInfo")]
    public object ListXmlRulesInfo { get; set; }

    [JsonPropertyName("LastTicketDate")]
    public DateTimeOffset? LastTicketDate { get; set; }

    [JsonPropertyName("Session")]
    public object Session { get; set; }

    [JsonPropertyName("AutoIssue")]
    public bool? AutoIssue { get; set; }

    [JsonPropertyName("CAcode")]
    public object CAcode { get; set; }

    [JsonPropertyName("VIPText")]
    public object VIPText { get; set; }

    [JsonPropertyName("Remark")]
    public object Remark { get; set; }

    [JsonPropertyName("AccountCode")]
    public object AccountCode { get; set; }

    [JsonPropertyName("VerifyMode")]
    public object VerifyMode { get; set; }

    [JsonPropertyName("VnaApiInfo")]
    public object VnaApiInfo { get; set; }
}

public sealed class FlightResponse
{
    [JsonPropertyName("Leg")]
    public int? Leg { get; set; }

    [JsonPropertyName("FlightId")]
    public int? FlightId { get; set; }

    [JsonPropertyName("Airline")]
    public string? Airline { get; set; }

    [JsonPropertyName("Operating")]
    public string? Operating { get; set; }

    [JsonPropertyName("StartPoint")]
    public string? StartPoint { get; set; }

    [JsonPropertyName("EndPoint")]
    public string? EndPoint { get; set; }

    [JsonPropertyName("StartDate")]
    public DateTimeOffset? StartDate { get; set; }

    [JsonPropertyName("EndDate")]
    public DateTimeOffset? EndDate { get; set; }

    [JsonPropertyName("StartDt")]
    public string? StartDt { get; set; }

    [JsonPropertyName("EndDt")]
    public string? EndDt { get; set; }

    [JsonPropertyName("FlightNumber")]
    public string? FlightNumber { get; set; }

    [JsonPropertyName("Duration")]
    public int? Duration { get; set; }

    [JsonPropertyName("StopNum")]
    public int? StopNum { get; set; }

    [JsonPropertyName("FlightValue")]
    public string? FlightValue { get; set; }

    [JsonPropertyName("QH_airBookingId")]
    public string? QHAirBookingId { get; set; }

    [JsonPropertyName("ListSegment")]
    public List<SegmentResponse> ListSegment { get; set; }

    [JsonPropertyName("HasDownStop")]
    public bool? HasDownStop { get; set; }

    [JsonPropertyName("NoRefund")]
    public bool? NoRefund { get; set; }

    [JsonPropertyName("GroupClass")]
    public string? GroupClass { get; set; }

    [JsonPropertyName("FareClass")]
    public string? FareClass { get; set; }

    [JsonPropertyName("FareBasis")]
    public string? FareBasis { get; set; }

    [JsonPropertyName("SeatRemain")]
    public int? SeatRemain { get; set; }

    [JsonPropertyName("Promo")]
    public bool? Promo { get; set; }
}

public sealed class SegmentResponse
{
    [JsonPropertyName("Id")]
    public int? Id { get; set; }

    [JsonPropertyName("Airline")]
    public string? Airline { get; set; }

    [JsonPropertyName("MarketingAirline")]
    public string? MarketingAirline { get; set; }

    [JsonPropertyName("OperatingAirline")]
    public string? OperatingAirline { get; set; }

    [JsonPropertyName("StartPoint")]
    public string? StartPoint { get; set; }

    [JsonPropertyName("EndPoint")]
    public string? EndPoint { get; set; }

    [JsonPropertyName("StartTime")]
    public DateTimeOffset? StartTime { get; set; }

    [JsonPropertyName("StartTimeZoneOffset")]
    public string? StartTimeZoneOffset { get; set; }

    [JsonPropertyName("EndTime")]
    public DateTimeOffset? EndTime { get; set; }

    [JsonPropertyName("EndTimeZoneOffset")]
    public string? EndTimeZoneOffset { get; set; }

    [JsonPropertyName("StartTm")]
    public string? StartTm { get; set; }

    [JsonPropertyName("EndTm")]
    public string? EndTm { get; set; }

    [JsonPropertyName("FlightNumber")]
    public string? FlightNumber { get; set; }

    [JsonPropertyName("Duration")]
    public int? Duration { get; set; }

    [JsonPropertyName("Plane")]
    public string? Plane { get; set; }

    [JsonPropertyName("StartTerminal")]
    public object StartTerminal { get; set; }

    [JsonPropertyName("EndTerminal")]
    public object EndTerminal { get; set; }

    [JsonPropertyName("HasStop")]
    public bool? HasStop { get; set; }

    [JsonPropertyName("StopPoint")]
    public string? StopPoint { get; set; }

    [JsonPropertyName("StopTime")]
    public int? StopTime { get; set; }

    [JsonPropertyName("DayChange")]
    public bool? DayChange { get; set; }

    [JsonPropertyName("StopOvernight")]
    public bool? StopOvernight { get; set; }

    [JsonPropertyName("ChangeStation")]
    public bool? ChangeStation { get; set; }

    [JsonPropertyName("ChangeAirport")]
    public bool? ChangeAirport { get; set; }

    [JsonPropertyName("LastItem")]
    public bool? LastItem { get; set; }

    [JsonPropertyName("MarriageGrp")]
    public object MarriageGrp { get; set; }

    [JsonPropertyName("FlightsMiles")]
    public int? FlightsMiles { get; set; }

    [JsonPropertyName("Status")]
    public string? Status { get; set; }

    [JsonPropertyName("Seat")]
    public int? Seat { get; set; }

    [JsonPropertyName("Cabin")]
    public string? Cabin { get; set; }

    [JsonPropertyName("Class")]
    public string? Class { get; set; }

    [JsonPropertyName("FareBasis")]
    public string? FareBasis { get; set; }

    [JsonPropertyName("HandBaggage")]
    public string? HandBaggage { get; set; }

    [JsonPropertyName("AllowanceBaggage")]
    public string? AllowanceBaggage { get; set; }

    [JsonPropertyName("ListHiddenStop")]
    public object ListHiddenStop { get; set; }

    [JsonPropertyName("QHFareInfo")]
    public object QHFareInfo { get; set; }
}