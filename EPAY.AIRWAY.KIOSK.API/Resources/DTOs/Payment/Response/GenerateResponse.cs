using EPAY.AIRWAY.KIOSK.API.Resources.Enums;

namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Payment.Response;

public sealed class GenerateResponse
{
    public PaymentType PaymentType { get; set; }
    
    public PlatformType PlatformType { get; set; }

    public string OrderCode { get; set; }

    public string BillCode { get; set; }

    public DateTime? ExpiredDatetimeUtc { get; set; }
    
    public DateTime? RequestDatetimeUtc { get; set; }
    
    public decimal TotalAmount { get; set; }
    public string? Qr { get; set; }

    public string? PaymentUrl { get; set; }
    public string? PaymentDeeplink { get; set; }
    public string? ReturnUrl { get; set; }
}