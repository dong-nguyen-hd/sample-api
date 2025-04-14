namespace AIRWAY.KIOSK.API.Resources.DTOs.HealthCheck.Response;

public sealed class ThirdPartyCheck
{
    public bool PaymentGateway { get; set; }
    public bool AbTrip { get; set; }
    public bool Database { get; set; }
    public bool SendEmail { get; set; }
}