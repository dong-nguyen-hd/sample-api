using EPAY.AIRWAY.KIOSK.API.Domain.Models.Base;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Models;

public sealed class Ticket : BaseModel
{
    public int Id { get; set; }
    public Model.Bill Bill { get; set; }
    public int BillId { get; set; }
}