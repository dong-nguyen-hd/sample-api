using EPAY.AIRWAY.KIOSK.API.Domain.Models.Base;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Models.ReportSection;

public sealed class SaleChannel : BaseModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string? Code { get; set; }
    
    public string? Name { get; set; }
    
    public HashSet<Model.ReportSection.ServicePartner>? ServicePartners { get; set; }
}