namespace EPAY.AIRWAY.KIOSK.API.Domain.Models.ToJson;

using Resources.Enums;

public sealed class Theme
{
    public PlatformType Type { get; set; }
    public string? CustomKey { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? BgColor { get; set; }
    public string? TextColor { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public HashSet<PaymentMethod>? PaymentMethods { get; set; }
}
