using EPAY.AIRWAY.KIOSK.API.Domain.Models.Base;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Models.ReportSection;

public sealed class ServicePartner  : BaseModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string? Code { get; set; }
    
    public string? Name { get; set; }
    
    public Guid SaleChannelId { get; set; }
    public Model.ReportSection.SaleChannel SaleChannel { get; set; }
    
    public HashSet<Model.ReportSection.Location>? Locations { get; set; }
    
    public HashSet<Model.ReportSection.Device>? Devices { get; set; }
}