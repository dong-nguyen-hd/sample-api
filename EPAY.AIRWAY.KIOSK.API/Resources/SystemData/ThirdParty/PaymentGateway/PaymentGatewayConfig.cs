namespace EPAY.AIRWAY.KIOSK.API.Resources.SystemData.ThirdParty.PaymentGateway;

public sealed class PaymentGatewayConfig
{
    public string? MerchantCode { get; set; }
    public string? Account { get; set; }
    public string? Password { get; set; }
    public string? PublicKeyForBe { get; set; }
    public string? PrivateKeyForBe { get; set; }
    public string? PublicKey { get; set; }
    public string? SecretKey { get; set; }
    public string? ClientIp { get; set; }

    /// <summary>
    /// Đơn vị: phút
    /// </summary>
    public int? TimeLimitQr { get; set; }

    /// <summary>
    /// Đơn vị: phút
    /// </summary>
    public int? TimeLimitCard { get; set; }

    /// <summary>
    /// Đơn vị: phút
    /// </summary>
    public int? TimeLimitBankAccount { get; set; }
    
    /// <summary>
    /// Đơn vị: phút
    /// </summary>
    public int? TimeLimitPos { get; set; }
    
    /// <summary>
    /// Đơn vị: phút
    /// </summary>
    public int? TimeLimitEpayWallet { get; set; }

    public string? OrderDescription { get; set; }
    public string? AgencyCode { get; set; }
}