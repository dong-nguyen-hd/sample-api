using EPAY.AIRWAY.KIOSK.API.Domain.Models.Base;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Models;

public sealed class Configuration : BaseModel
{
    public int Id { get; set; }
    public string Key { get; set; }
    public string? Value { get; set; }
    public bool Internal { get; set; }
}
