using AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;

namespace AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.IACV.Response;

public class QueryDataResponse
{
    public List<FlightInformationResponse?>? Items { get; set; }
}

public class FlightInformationResponse
{
    /// <summary>
    /// Mã: AbTripOrderId
    /// </summary>
    public string? OrderId { get; set; }
    
    /// <summary>
    /// Ngày đặt hàng
    /// </summary>
    public DateTime? OrderDatetimeUtc { get; set; }
    
    /// <summary>
    /// Loại hành trình
    /// </summary>
    public MyEnum.TicketType TicketType { get; set; }
    
    /// <summary>
    /// Thông tin chiều bay khời hành
    /// </summary>
    public FlightPointResponse? FlightStart { get; set; }
    
    /// <summary>
    /// Thông tin chiều bay kết thúc
    /// </summary>
    public FlightPointResponse? FlightEnd { get; set; }
    
    /// <summary>
    /// Thông tin thanh toán
    /// </summary>
    public PaymentResponse? PaymentInformation { get; set; }
}

public class FlightPointResponse
{
    public string? BookingCode { get; set; }
    
    /// <summary>
    /// Thông tin điểm đi
    /// </summary>
    public AirportResponse? StartPoint { get; set; }
    
    /// <summary>
    /// Thông tin điểm dừng
    /// </summary>
    public AirportResponse? EndPoint { get; set; }
    
    public DateTime? StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    public string? FlightNumber { get; set; }
    
    public AirlineResponse? Airline { get; set; }
}

public class PaymentResponse
{
    public MyEnum.PaymentType PaymentType { get; set; }
    
    public string? OrderCode { get; set; }
    
    /// <summary>
    /// Thời gian thanh toán
    /// </summary>
    public DateTime? PaidDatetimeUtc { get; set; }
    
    /// <summary>
    /// Tổng tiền thanh toán
    /// </summary>
    public int TotalAmount { get; set; }
    
    /// <summary>
    /// Thời hạn đơn hàng có thể thanh toán, áp dụng với đơn trả sau
    /// </summary>
    public DateTime? ExpiredDatetimeUtc { get; set; }
    
    /// <summary>
    /// Đường dẫn thông tin trả sau
    /// </summary>
    public string? PaylaterUri { get; set; }
}