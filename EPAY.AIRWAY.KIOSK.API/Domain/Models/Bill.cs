using EPAY.AIRWAY.KIOSK.API.Domain.Models.Base;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Models;

/// <summary>
/// Thông tin hoá đơn mua hàng
/// </summary>
public sealed class Bill : BaseModel
{
    public string Id { get; set; } = RelateText.GenId();

    /// <summary>
    /// Mã "BookingId" trả về từ api booking abtrip
    /// </summary>
    public string? BookingId { get; set; }
    
    /// <summary>
    /// Mã "OrderCode" trả về từ api booking abtrip
    /// </summary>
    public string? OrderCode { get; set; }

    /// <summary>
    /// Tổng giá trị đơn hàng
    /// </summary>
    public int TotalPrice { get; set; }

    public Invoice? Invoice { get; set; }
    public Contact? Contact { get; set; }
    public HashSet<Model.Reservation>? Reservations { get; set; }
    public HashSet<Model.Passenger>? Passengers { get; set; }
    public HashSet<Model.PaymentTransaction>? PaymentTransactions { get; set; }
}