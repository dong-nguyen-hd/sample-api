namespace AIRWAY.KIOSK.API.Resources.Enums;

public enum OrderType : byte
{
    /// <summary>
    /// Thời gian bay chiều đi tăng dần
    /// </summary>
    FlightStartAsc = 2,
    
    /// <summary>
    /// Thời gian bay chiều đi giảm dần
    /// </summary>
    FlightStartDesc = 3,
    
    /// <summary>
    /// Thời gian bay chiều về tăng dần
    /// </summary>
    FlightEndAsc = 4,
    
    /// <summary>
    /// Thời gian bay chiều về giảm dần
    /// </summary>
    FlightEndDesc = 5,
    
    /// <summary>
    /// Thời gian đặt đơn hàng tăng dần
    /// </summary>
    OrderAsc = 6,
    
    /// <summary>
    /// Thời gian đặt đơn hàng giảm dần
    /// </summary>
    OrderDesc = 7,
}