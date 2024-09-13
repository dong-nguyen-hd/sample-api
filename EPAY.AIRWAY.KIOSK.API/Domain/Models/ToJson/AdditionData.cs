namespace EPAY.AIRWAY.KIOSK.API.Domain.Models.ToJson;

public sealed record AdditionData
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public HashSet<Theme>? Themes { get; set; }
}
