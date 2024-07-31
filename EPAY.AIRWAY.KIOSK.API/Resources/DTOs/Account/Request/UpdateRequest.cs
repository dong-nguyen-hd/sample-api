using Models.ToJson;

namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Account.Request;

public sealed class UpdateRequest
{
    public string? UserName { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public List<string> SystemRoles { get; set; }
    public AdditionData? AdditionData { get; set; }
}
