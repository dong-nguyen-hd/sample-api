using AIRWAY.KIOSK.API.Domain.Models.Base;

namespace AIRWAY.KIOSK.API.Domain.Models;

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
    /// Phân loại hành khách
    /// </summary>
    public MyEnum.PassengerType? PassengerType { get; set; }
    
    /// <summary>
    /// Tổng giá trên một vé
    /// </summary>
    public int? TotalPrice { get; set; }

    /// <summary>
    /// Thời gian xuất vé
    /// </summary>
    public DateTime? IssueDatetimeUtc { get; set; }

    public string BillId { get; set; } = null!;
    public Model.Bill Bill { get; set; }
}