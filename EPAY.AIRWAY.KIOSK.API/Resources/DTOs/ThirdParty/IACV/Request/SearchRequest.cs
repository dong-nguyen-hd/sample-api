using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Pagination.Request;

namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.IACV.Request;

public sealed class SearchRequest : FilterRequest
{
    
    /// <summary>
    /// Mã đơn hàng [AbtripOrderId]
    /// </summary>
    public string? OrderId { get; set; }

    /// <summary>
    /// Tên điểm đến
    /// </summary>
    public string? PointName { get; set; }
    
    /// <summary>
    /// Thời gian bắt đầu tạo đơn
    /// </summary>
    public DateOnly? StartDate { get; set; }
    
    /// <summary>
    /// Thời gian kết thúc tạo đơn
    /// </summary>
    public DateOnly? EndDate { get; set; }
}