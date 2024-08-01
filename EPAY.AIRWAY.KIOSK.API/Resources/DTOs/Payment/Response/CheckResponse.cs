using EPAY.AIRWAY.KIOSK.API.Resources.Enums;

namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Payment.Response;

public sealed class CheckResponse
{
    public PaymentType PaymentType { get; set; }
    
    public PlatformType PlatformType { get; set; }

    public string OrderCode { get; set; }

    public string BillId { get; set; }

    public PaymentStatus PaymentStatus { get; set; }

    public DateTime? ExpiredDatetimeUtc { get; set; }
    
    public decimal TotalAmount { get; set; }
}