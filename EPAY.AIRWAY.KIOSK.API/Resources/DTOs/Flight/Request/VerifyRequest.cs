namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request;

public sealed class VerifyRequest
{
    public int? TotalPrice { get; set; }
    public List<FareDataRequest>? ListFareData { get; set; }
}