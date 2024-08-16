using EPAY.AIRWAY.KIOSK.API.Domain.Models.Base;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Models;

public sealed class Contact : BaseModel
{
    public string Id { get; set; } = RelateText.GenId();
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public bool Gender { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public DateTime? BirthDay { get; set; }
    
    public string BillId { get; set; }
    public Bill Bill { get; set; } = null!;
}