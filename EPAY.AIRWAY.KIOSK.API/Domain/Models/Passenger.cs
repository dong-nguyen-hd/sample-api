using EPAY.AIRWAY.KIOSK.API.Domain.Models.Base;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Models;

public sealed class Passenger : BaseModel
{
    public string Id { get; set; } = RelateText.GenId();

    public string BillId { get; set; }
    public Bill Bill { get; set; } = null!;
    public HashSet<Model.AdditionalService>? AdditionalServices { get; set; }
}