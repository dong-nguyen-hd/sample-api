using AIRWAY.KIOSK.API.Domain.Models.ToJson;

namespace AIRWAY.KIOSK.API.Resources.DTOs.Account.Response;

public class AccountResponse
{
    public string? Id { get; set; }
    public string? UserName { get; set; }
    public string? Name { get; set; }
    public string? Avatar { get; set; }
    public string? Email { get; set; }
    public List<string>? SystemRoles { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public AdditionData? AdditionData { get; set; }
    public DateTime? CreatedDatetime { get; set; }
    public DateTime? UpdatedDatetime { get; set; }
}
