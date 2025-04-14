using AbTrip = AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip;

namespace AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request;

public sealed class GroupDataRequest
{
    /// <summary>
    /// Điểm khởi hành
    /// </summary>
    public string? StartPoint { get; set; }
    
    /// <summary>
    /// Điểm kết thúc
    /// </summary>
    public string? EndPoint { get; set; }
    
    /// <summary>
    /// Danh sách dữ liệu chuyến bay
    /// </summary>
    public List<GroupInnerDataRequest>? DetectFlight { get; set; }
}

public sealed class GroupInnerDataRequest
{
    /// <summary>
    /// Số hiệu chuyến bay
    /// </summary>
    public string? FlightNumber { get; set; }
    
    /// <summary>
    /// Ngày khởi hành
    /// </summary>
    public DateTime? StartDate { get; set; }
    
    /// <summary>
    /// Danh sách fare
    /// </summary>
    public List<AbTrip.Response.SearchFlightInner>? FareData { get; set; }
}