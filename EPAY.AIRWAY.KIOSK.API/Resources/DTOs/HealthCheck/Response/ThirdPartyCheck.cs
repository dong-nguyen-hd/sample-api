namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.HealthCheck.Response;

public sealed class ThirdPartyCheck
{
    public bool PaymentGateway { get; set; }
    public bool AbTrip { get; set; }
    public bool System { get; set; }
}