using EPAY.AIRWAY.KIOSK.API.Domain.Models.Base;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Models.ReportSection;

public sealed class Device : BaseModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string Code { get; set; } = null!;
    
    public string? Serial { get; set; }
    
    public string? PosSerial { get; set; }

    public string? PosRefId { get; set; }

    public string? PosMerchantId { get; set; }

    public string? PosClientId { get; set; }

    public string? PosMerchantOutletId { get; set; }

    public string? PosTerminalId { get; set; }
    
    public Guid ServicePartnerId { get; set; }
    public Model.ReportSection.ServicePartner ServicePartner { get; set; }
}