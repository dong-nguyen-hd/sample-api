namespace AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip.Response;

public sealed class GetFareRulesResponse : BaseResponse
{
    [JsonPropertyName("ListFareRules")]
    public List<FareRuleResponse>? ListFareRules { get; set; }
}

public sealed class FareRuleResponse
{
    [JsonPropertyName("Route")]
    public string? Route { get; set; }

    [JsonPropertyName("FareBasis")]
    public string? FareBasis { get; set; }

    [JsonPropertyName("ListRulesGroup")]
    public List<RulesGroupResponse>? ListRulesGroup { get; set; }

    [JsonPropertyName("FareDataInfo")]
    public FareDataInfoResponse? FareDataInfo { get; set; }
}

public sealed class RulesGroupResponse
{
    [JsonPropertyName("RulesTitle")]
    public string? RulesTitle { get; set; }

    [JsonPropertyName("ListRulesText")]
    public List<string>? ListRulesText { get; set; }
}


public sealed class FareDataInfoResponse
{
    [JsonPropertyName("Session")]
    public string? Session { get; set; }

    [JsonPropertyName("FareDataId")]
    public int? FareDataId { get; set; }
}