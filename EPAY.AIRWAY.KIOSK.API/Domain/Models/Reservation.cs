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
    /// 
    /// </summary>
    public DateTime? ExpiryDate { get; set; }
    
    /// <summary>
    /// Điểm khởi hành
    /// </summary>
    public string? StartPoint { get; set; }
    
    /// <summary>
    /// Điểm kết thúc
    /// </summary>
    public string? EndPoint { get; set; }
    
    /// <summary>
    /// Hãng hàng không thực hiện booking
    /// </summary>
    public string? Airline { get; set; }
    
    /// <summary>
    /// Giá trị chuyến bay
    /// </summary>
    public string? FlightValue { get; set; }
    
    /// <summary>
    /// Session
    /// </summary>
    public string? Session { get; set; }
    
    /// <summary>
    /// Tổng giá cho booking
    /// </summary>
    public int? TotalPrice { get; set; }
    
    /// <summary>
    /// Số lượng người lớn
    /// </summary>
    public int? Adt { get; set; }
    
    /// <summary>
    /// Số lượng trẻ nhỏ
    /// </summary>
    public int? Chd { get; set; }
    
    /// <summary>
    /// Số lượng em bé
    /// </summary>
    public int? Inf { get; set; }
    
    /// <summary>
    /// Danh sách fare-id
    /// </summary>
    public List<string?>? FareDataIds { get; set; }

    public string BillId { get; set; } = null!;
    public Model.Bill Bill { get; set; }
}