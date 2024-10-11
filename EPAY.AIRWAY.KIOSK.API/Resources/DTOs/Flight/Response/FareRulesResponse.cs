namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;

public sealed class FareRulesResponse
{
    public string? FareBasis { get; set; }
    public List<string?>? ListRulesGroup { get; set; }
}