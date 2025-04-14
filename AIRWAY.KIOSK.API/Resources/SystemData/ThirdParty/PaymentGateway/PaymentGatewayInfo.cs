namespace AIRWAY.KIOSK.API.Resources.SystemData.ThirdParty.PaymentGateway;

public sealed record PaymentGatewayInfo
{
    public PaymentGatewayConfig? Config { get; set; }
    public PaymentGatewayApi? Api { get; set; }
}