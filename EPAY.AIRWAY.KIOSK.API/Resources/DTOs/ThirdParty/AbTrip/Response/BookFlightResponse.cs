namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip.Response;

public sealed class BookFlightResponse : BaseResponse
{
    [JsonPropertyName("BookingId")]
    public int? BookingId { get; set; }

    [JsonPropertyName("OrderCode")]
    public string? OrderCode { get; set; }

    [JsonPropertyName("ListBooking")]
    public List<BookingResponse> ListBooking { get; set; }
}

public sealed class BookingResponse
{
    [JsonPropertyName("Status")]
    public string? Status { get; set; }

    [JsonPropertyName("System")]
    public string? System { get; set; }

    [JsonPropertyName("Source")]
    public string? Source { get; set; }

    [JsonPropertyName("Airline")]
    public string? Airline { get; set; }

    [JsonPropertyName("BookingCode")]
    public string? BookingCode { get; set; }

    [JsonPropertyName("GdsCode")]
    public string? GdsCode { get; set; }

    [JsonPropertyName("Flight")]
    public string? Flight { get; set; }

    [JsonPropertyName("Route")]
    public string? Route { get; set; }

    [JsonPropertyName("AutoIssue")]
    public bool? AutoIssue { get; set; }

    [JsonPropertyName("ErrorCode")]
    public string? ErrorCode { get; set; }

    [JsonPropertyName("ErrorMessage")]
    public string? ErrorMessage { get; set; }

    [JsonPropertyName("BookingImage")]
    public object BookingImage { get; set; }

    [JsonPropertyName("ExpiryDate")]
    public DateTime? ExpiryDate { get; set; }

    [JsonPropertyName("ExpiryDt")]
    public string? ExpiryDt { get; set; }

    [JsonPropertyName("ExpiryTime")]
    public int? ExpiryTime { get; set; }

    [JsonPropertyName("TimePurchase")]
    public DateTime? TimePurchase { get; set; }

    [JsonPropertyName("PriceExpiration")]
    public string? PriceExpiration { get; set; }

    [JsonPropertyName("ResponseTime")]
    public double? ResponseTime { get; set; }

    [JsonPropertyName("Price")]
    public int? Price { get; set; }

    [JsonPropertyName("Difference")]
    public int? Difference { get; set; }

    [JsonPropertyName("ListTicket")]
    public object ListTicket { get; set; }

    [JsonPropertyName("Warnings")]
    public object Warnings { get; set; }

    [JsonPropertyName("Session")]
    public string? Session { get; set; }

    [JsonPropertyName("ListPassenger")]
    public List<BookingPassengerResponse> ListPassenger { get; set; }

    [JsonPropertyName("ListFareData")]
    public List<BookingFareDataResponse> ListFareData { get; set; }

    [JsonPropertyName("FareData")]
    public FareDataResponse FareData { get; set; }
}

public sealed class FareDataResponse
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
    public object Tourcode { get; set; }

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
    public List<BookingFlightResponse> ListFlight { get; set; }

    [JsonPropertyName("ListXmlRulesInfo")]
    public object ListXmlRulesInfo { get; set; }

    [JsonPropertyName("LastTicketDate")]
    public DateTime? LastTicketDate { get; set; }

    [JsonPropertyName("Session")]
    public string? Session { get; set; }

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

public sealed class BookingFareResponse
{
    [JsonPropertyName("DocType")]
    public string? DocType { get; set; }

    [JsonPropertyName("BaseFare")]
    public int? BaseFare { get; set; }

    [JsonPropertyName("Fees")]
    public int? Fees { get; set; }

    [JsonPropertyName("Taxes")]
    public int? Taxes { get; set; }

    [JsonPropertyName("VAT")]
    public int? VAT { get; set; }
}

public sealed class BookingFareDataResponse
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
    public object Tourcode { get; set; }

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
    public List<BookingFlightResponse> ListFlight { get; set; }

    [JsonPropertyName("ListXmlRulesInfo")]
    public object ListXmlRulesInfo { get; set; }

    [JsonPropertyName("LastTicketDate")]
    public DateTime? LastTicketDate { get; set; }

    [JsonPropertyName("Session")]
    public string? Session { get; set; }

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

public sealed class BookingFlightResponse
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
    public DateTime? StartDate { get; set; }

    [JsonPropertyName("EndDate")]
    public DateTime? EndDate { get; set; }

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
    public object QHAirBookingId { get; set; }

    [JsonPropertyName("ListSegment")]
    public List<BookingSegmentResponse> ListSegment { get; set; }

    [JsonPropertyName("ListPrice")]
    public List<object> ListPrice { get; set; }

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

public sealed class BookingPassengerResponse
{
    [JsonPropertyName("Index")]
    public int? Index { get; set; }

    [JsonPropertyName("NameId")]
    public object NameId { get; set; }

    [JsonPropertyName("ParentId")]
    public int? ParentId { get; set; }

    [JsonPropertyName("FirstName")]
    public string? FirstName { get; set; }

    [JsonPropertyName("LastName")]
    public string? LastName { get; set; }

    [JsonPropertyName("Type")]
    public string? Type { get; set; }

    [JsonPropertyName("Gender")]
    public bool? Gender { get; set; }

    [JsonPropertyName("Birthday")]
    public string? Birthday { get; set; }

    [JsonPropertyName("PassportNumber")]
    public string? PassportNumber { get; set; }

    [JsonPropertyName("PassportExpiry")]
    public string? PassportExpiry { get; set; }

    [JsonPropertyName("Nationality")]
    public string? Nationality { get; set; }

    [JsonPropertyName("IssueCountry")]
    public string? IssueCountry { get; set; }

    [JsonPropertyName("Membership")]
    public object Membership { get; set; }

    [JsonPropertyName("CustLoyalty")]
    public object CustLoyalty { get; set; }

    [JsonPropertyName("ListFare")]
    public List<BookingFareResponse> ListFare { get; set; }

    [JsonPropertyName("ListBaggage")]
    public List<object> ListBaggage { get; set; }

    [JsonPropertyName("ListSeat")]
    public List<object> ListSeat { get; set; }

    [JsonPropertyName("ListService")]
    public List<object> ListService { get; set; }

    [JsonPropertyName("ListCustLoyalty")]
    public object ListCustLoyalty { get; set; }

    [JsonPropertyName("NewPassenger")]
    public object NewPassenger { get; set; }
}

public sealed class BookingSegmentResponse
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
    public DateTime? StartTime { get; set; }

    [JsonPropertyName("StartTimeZoneOffset")]
    public string? StartTimeZoneOffset { get; set; }

    [JsonPropertyName("EndTime")]
    public DateTime? EndTime { get; set; }

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
    public object StopPoint { get; set; }

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

    [JsonPropertyName("QHItinerary")]
    public object QHItinerary { get; set; }

    [JsonPropertyName("QHFareInfo")]
    public object QHFareInfo { get; set; }
}