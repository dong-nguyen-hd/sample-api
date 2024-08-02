namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;

public sealed class FareRulesResponse
{
    public string? FareBasis { get; set; }
    public List<RulesGroupResponse>? ListRulesGroup { get; set; }
}

public sealed class RulesGroupResponse
{
    public string? RulesTitle { get; set; }
    public List<string>? ListRulesText { get; set; }
}