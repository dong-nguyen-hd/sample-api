using EPAY.AIRWAY.KIOSK.API.Domain.Models.Base;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Models;

/// <summary>
/// Thông tin hoá đơn mua hàng
/// </summary>
public sealed class Bill : BaseModel
{
    public string Id { get; set; } = RelateText.GenId();
    
    /// <summary>
    /// Loại chuyến bay
    /// </summary>
    public MyEnum.FlightType FlightType { get; set; }
    
    /// <summary>
    /// Xác định hoá đơn phát sính từ hệ thống thứ khác: <br/>
    /// true - hệ thống khác <br/>
    /// false - hệ thống epay <br/>
    /// </summary>
    public bool IsThirdParty { get; set; }
    
    /// <summary>
    /// Thời gian hoá đơn hết hiệu lực
    /// </summary>
    public DateTime ExpiredDatetimeUtc { get; set; }
    
    /// <summary>
    /// Timezone của ExpiredDatetimeUtc trong trường hợp muốn chuyển localtime
    /// </summary>
    public string? StartTimeZoneOffset { get; set; }
    
    /// <summary>
    /// Mã giao dịch trao đổi giữa Abtrip và Epay
    /// </summary>
    public string? AbTripOrderId { get; set; }
    
    /// <summary>
    /// Mã "BookingId" trả về từ api booking abtrip
    /// </summary>
    public string? AbTripBookingId { get; set; }
    
    /// <summary>
    /// Mã "OrderCode" trả về từ api booking abtrip
    /// </summary>
    public string? AbTripOrderCode { get; set; }

    /// <summary>
    /// Tổng giá trị đơn hàng
    /// </summary>
    public int TotalPrice { get; set; }

    public Invoice? Invoice { get; set; }
    public Contact? Contact { get; set; }
    public HashSet<Model.Ticket>? Tickets { get; set; }
    public HashSet<Model.Reservation>? Reservations { get; set; }
    public HashSet<Model.Passenger>? Passengers { get; set; }
    public HashSet<Model.PaymentTransaction>? PaymentTransactions { get; set; }
    public HashSet<Model.FareData>? FareDatas { get; set; }
    public HashSet<Model.FlightData>? FlightDatas { get; set; }
}