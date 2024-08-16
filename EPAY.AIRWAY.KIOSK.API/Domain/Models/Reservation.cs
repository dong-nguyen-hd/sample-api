using EPAY.AIRWAY.KIOSK.API.Domain.Models.Base;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Models;

public sealed class Reservation : BaseModel
{
    public string Id { get; set; } = RelateText.GenId();

    public string BillId { get; set; } = null!;
    public Model.Bill Bill { get; set; }
}