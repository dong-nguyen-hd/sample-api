using AIRWAY.KIOSK.API.Domain.Models.Base;

namespace AIRWAY.KIOSK.API.Domain.Models;

/// <summary>
/// Tuỳ chỉnh thiết lập cron job
/// </summary>
public class CronJobFlag : BaseModel
{
    public uint Version { get; set; }
    
    public string Id { get; set; } = RelateText.GenId();

    public string? Name { get; set; }
}