using AIRWAY.KIOSK.API.Resources.Enums;

namespace AIRWAY.KIOSK.API.Resources.DTOs.Payment.Response;

public sealed class GenerateResponse
{
    public PaymentType PaymentType { get; set; }

    public PlatformType PlatformType { get; set; }

    public string? OrderCode { get; set; }

    public string? BillId { get; set; }

    public DateTime? ExpiredDatetimeUtc { get; set; }
    
    public int? SecondsExpiration { get; set; }

    public DateTime? RequestDatetimeUtc { get; set; }

    public int TotalAmount { get; set; }
    
    public string? Qr { get; set; }

    public string? PaymentUrl { get; set; }
    
    public string? PaymentDeeplink { get; set; }
    
    public string? ReturnUrl { get; set; }
}