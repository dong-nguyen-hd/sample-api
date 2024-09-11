using EPAY.AIRWAY.KIOSK.API.Domain.Models.Base;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Models;

/// <summary>
/// Thônng tin đặt chỗ
/// </summary>
public sealed class Reservation : BaseModel
{
    public string Id { get; set; } = RelateText.GenId();

    /// <summary>
    /// Mã đặt chỗ
    /// </summary>
    public string? BookingCode { get; set; }
    
    /// <summary>
    /// GdsCode
    /// </summary>
    public string? GdsCode { get; set; }
    
    /// <summary>
    /// Thời gian hết hạn booking
    /// </summary>
    public DateTime? ExpiryDate { get; set; }
    
    /// <summary>
    /// Hãng hàng không thực hiện booking
    /// </summary>
    public string? Airline { get; set; }
    
    /// <summary>
    /// Giá trị chuyến bay <br/>
    /// Lấy thông tin từ ListBooking:Flight
    /// </summary>
    public string? FlightValue { get; set; }
    
    /// <summary>
    /// Chiều di chuyển chuyến bay
    /// </summary>
    public string? Route { get; set; }
    
    /// <summary>
    /// Session
    /// </summary>
    public string? Session { get; set; }
    
    /// <summary>
    /// Trạng thái booking đã được xuất vé <br/>
    /// true - đã xuất vé thành công
    /// false -  chưa xuất vé
    /// </summary>
    public bool TicketIssued { get; set; }

    public string BillId { get; set; } = null!;
    public Model.Bill Bill { get; set; }
}