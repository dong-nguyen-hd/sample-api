namespace EPAY.AIRWAY.KIOSK.API.Resources.SystemData.ThirdParty.AbTrip;

public sealed record AbTripApi
{
    public bool EnableVerifyTls { get; set; }
    public string? BaseAddress { private get; set; }
    public string? SearchFlight { private get; set; }
    public string? Baggage { private get; set; }
    public string? FareRules { private get; set; }
    public string? VerifyFlight { private get; set; }
    public string? PriceQuote { private get; set; }
    public string? BookFlight { private get; set; }
    public string? Aircrafts { private get; set; }
    public string? Airports { private get; set; }
    public string? Airlines { private get; set; }

    #region Method

    public string GetSearchFlightUri() =>
        $"{BaseAddress}/{SearchFlight}";

    public string GetBaggageUri() =>
        $"{BaseAddress}/{Baggage}";

    public string GetFareRulesUri() =>
        $"{BaseAddress}/{FareRules}";

    public string GetVerifyFlightUri() =>
        $"{BaseAddress}/{VerifyFlight}";

    public string GetPriceQuoteUri() =>
        $"{BaseAddress}/{PriceQuote}";

    public string GetBookFlightUri() =>
        $"{BaseAddress}/{BookFlight}";

    public string GetAircraftsUri() =>
        $"{BaseAddress}/{Aircrafts}";

    public string GetAirportsUri() =>
        $"{BaseAddress}/{Airports}";

    public string GetAirlinesUri() =>
        $"{BaseAddress}/{Airlines}";

    #endregion
}