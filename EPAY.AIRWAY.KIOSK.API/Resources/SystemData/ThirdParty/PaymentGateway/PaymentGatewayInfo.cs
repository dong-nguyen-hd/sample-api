namespace EPAY.AIRWAY.KIOSK.API.Resources.SystemData.ThirdParty.PaymentGateway;

public sealed class PaymentGatewayInfo
{
    public PaymentGatewayConfig Config { get; set; }
    public PaymentGatewayApi Api { get; set; }
}