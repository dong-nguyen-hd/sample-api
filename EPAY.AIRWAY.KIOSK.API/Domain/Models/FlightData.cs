using EPAY.AIRWAY.KIOSK.API.Domain.Models.Base;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Models;

public class FlightData : BaseModel
{
    public string Id { get; set; } = RelateText.GenId();

    /// <summary>
    /// Mã đặt chỗ
    /// </summary>
    public string? BookingCode { get; set; }
    
    public string? AbTripFareDataId { get; set; }
    
    /// <summary>
    /// Xác định chuyến bay khởi hành <br/>
    /// true - là chuyến khởi hành <br/>
    /// false - không là chuyến khởi hành
    /// </summary>
    public bool Departure { get; set; }
    
    public string? Airline { get; set; }
    
    public string? Operating { get; set; }
    
    public string? FlightId { get; set; }
    
    public string? StartPoint { get; set; }
    public DateTime? StartDate { get; set; }
    
    public string? EndPoint { get; set; }
    public DateTime? EndDate { get; set; }
    
    public string? FlightValue { get; set; }
    public string? FlightNumber { get; set; }
    
    public string BillId { get; set; } = null!;
    public Model.Bill Bill { get; set; }
}