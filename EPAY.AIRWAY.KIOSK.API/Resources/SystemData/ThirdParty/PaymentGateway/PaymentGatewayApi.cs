namespace EPAY.AIRWAY.KIOSK.API.Resources.SystemData.ThirdParty.PaymentGateway;

public sealed class PaymentGatewayApi
{
    public bool EnableVerifyTls { get; set; }
    public string? BaseAddress { private get; set; }
    public string? Refund { private get; set; }
    public string? CreateOrder { private get; set; }
    public string? CheckStatus { private get; set; }
    public string? Login { private get; set; }

    #region Method

    public string GetRefundUri() =>
        $"{BaseAddress}/{Refund}";

    public string GetCreateOrderUri() =>
        $"{BaseAddress}/{CreateOrder}";

    public string GetCheckStatusUri() =>
        $"{BaseAddress}/{CheckStatus}";

    public string GetLoginUri() =>
        $"{BaseAddress}/{Login}";

    #endregion
}