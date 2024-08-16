using EPAY.AIRWAY.KIOSK.API.Domain.Models.Base;
using Models.ToJson;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Models;

public sealed class Account : BaseModel
{
    public string Id { get; set; } = RelateText.GenId();
    public string UserName { get; set; }
    public string Password { get; set; }
    public string? Name { get; set; }
    public string? Avatar { get; set; }
    public string? Email { get; set; }
    public AdditionData? AdditionData { get; set; }
    public HashSet<RefreshToken>? RefreshTokens { get; set; }
    public List<string> SystemRoles { get; set; }
}
