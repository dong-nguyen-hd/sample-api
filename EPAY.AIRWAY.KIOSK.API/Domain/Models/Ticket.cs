using EPAY.AIRWAY.KIOSK.API.Domain.Models.Base;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Models;

/// <summary>
/// Thông tin vé
/// </summary>
public sealed class Ticket : BaseModel
{
    public string Id { get; set; } = RelateText.GenId();

    /// <summary>
    /// Mã đặt chỗ
    /// </summary>
    public string? BookingCode { get; set; }
    
    /// <summary>
    /// Mã vé
    /// </summary>
    public string? TicketNumber { get; set; }

    /// <summary>
    /// Thời gian xuất vé
    /// </summary>
    public DateTime? IssueDatetimeUtc { get; set; }

    public string BillId { get; set; } = null!;
    public Model.Bill Bill { get; set; }
}