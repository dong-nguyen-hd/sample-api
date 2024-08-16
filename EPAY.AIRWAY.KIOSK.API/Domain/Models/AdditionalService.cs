using EPAY.AIRWAY.KIOSK.API.Domain.Models.Base;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Models;

public sealed class AdditionalService : BaseModel
{
    public string Id { get; set; } = RelateText.GenId();
    public string StartPoint { get; set; } = null!;
    public string EndPoint { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string Currency { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Price { get; set; } = null!;
    public string Value { get; set; } = null!;

    public int PassengerId { get; set; }
    public Passenger Passenger { get; set; } = null!;
}