using AIRWAY.KIOSK.API.Domain.Models.Base;
using AIRWAY.KIOSK.API.Domain.Models.ReportSection.ToJson;

namespace AIRWAY.KIOSK.API.Domain.Models.ReportSection;

public sealed class Report : BaseModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? OrderCode { get; set; }
    public string? BillCode { get; set; }
    public string? PaymentType { get; set; }
    public string? PartnerPaymentType { get; set; }
    public string? PartnerPaymentStatus { get; set; }
    public string? TransCode { get; set; }
    public string? DeliveryStatus { get; set; }
    public long? PaymentAmount { get; set; }
    public string? SaleChannelCode { get; set; }
    public string? ServicePartnerCode { get; set; }
    public string? LocationCode { get; set; }
    public string? DeviceSerial { get; set; }
    public OtherInfo? OtherInfo { get; set; }
}