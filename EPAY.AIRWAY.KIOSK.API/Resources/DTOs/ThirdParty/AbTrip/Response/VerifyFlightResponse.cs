namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip.Response;

public sealed class VerifyFlightResponse : BaseResponse
{
    [JsonPropertyName("ListFareStatus")]
    public List<VerifyFlightInnerResponse>? ListFareStatus { get; set; }
}

public sealed class VerifyFlightInnerResponse
{
    [JsonPropertyName("Status")]
    public bool? Status { get; set; }

    [JsonPropertyName("Remark")]
    public string? Remark { get; set; }

    [JsonPropertyName("Price")]
    public int? Price { get; set; }

    [JsonPropertyName("Difference")]
    public int? Difference { get; set; }

    [JsonPropertyName("Session")]
    public string? Session { get; set; }
    
    [JsonPropertyName("FareData")]
    public SearchFlightInner? FareData { get; set; }
}