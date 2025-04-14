namespace AIRWAY.KIOSK.API.Resources.DTOs.Payment.Request;

public sealed class SavePaylaterRequest
{
    /// <summary>
    /// Mã định danh cho mỗi đơn hàng
    /// </summary>
    public string? BillId { get; set; }

    /// <summary>
    /// Kênh bán hàng, được hiểu là nền tảng tạo giao dịch đơn hàng
    /// </summary>
    public MyEnum.PlatformType PlatformType { get; set; }
}