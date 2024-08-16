using EPAY.AIRWAY.KIOSK.API.Domain.Models.Base;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Models;

public sealed class Configuration : BaseModel
{
    public string Id { get; set; } = RelateText.GenId();
    public string Key { get; set; }
    public string? Value { get; set; }
    public bool Internal { get; set; }
}