using AbTrip = EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip;

namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request;

public sealed class GroupDataRequest
{
    /// <summary>
    /// Chiều di chuyển
    /// </summary>
    public string? Way { get; set; }
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