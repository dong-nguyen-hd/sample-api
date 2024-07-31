using EPAY.AIRWAY.KIOSK.API.Domain.Models.Base;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Models;

public sealed class RefreshToken : BaseModel
{
    public string Id { get; set; }
    public string Token { get; set; }
    public DateTime ExpiredUtc { get; set; }
    public string? UserAgent { get; set; }
    public bool IsUsed { get; set; }
    public int AccountId { get; set; }
    public Account Account { get; set; }
}
