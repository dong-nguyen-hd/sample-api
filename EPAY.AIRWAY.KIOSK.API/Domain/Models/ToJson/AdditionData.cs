using EPAY.AIRWAY.KIOSK.API.Domain.Models.ToJson;

namespace Models.ToJson;

public class AdditionData
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public HashSet<Theme>? Themes { get; set; }
}
