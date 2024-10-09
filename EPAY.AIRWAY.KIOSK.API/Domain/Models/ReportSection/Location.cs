using EPAY.AIRWAY.KIOSK.API.Domain.Models.Base;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Models.ReportSection;

public class Location : BaseModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string? Code { get; set; }
    
    public string? Name { get; set; }
    
    public string? Address { get; set; }
    public string? City { get; set; }
    
    public Guid ServicePartnerId { get; set; }
    public Model.ReportSection.ServicePartner ServicePartner { get; set; }
}