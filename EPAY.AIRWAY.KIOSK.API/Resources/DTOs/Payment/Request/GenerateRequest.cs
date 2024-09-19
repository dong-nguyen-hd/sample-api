using EPAY.AIRWAY.KIOSK.API.Resources.Enums;

namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Payment.Request;

public sealed class GenerateRequest
{
    /// <summary>
    /// Phương thức thực hiện giao dịch
    /// </summary>
    public PaymentType PaymentType { get; set; }

    /// <summary>
    /// Mã định danh cho mỗi đơn hàng
    /// </summary>
    public string? BillId { get; set; }

    /// <summary>
    /// Kênh bán hàng, được hiểu là nền tảng tạo giao dịch đơn hàng
    /// </summary>
    public PlatformType PlatformType { get; set; }
    
    /// <summary>
    /// Url trả về màn hình kết quả thanh toán của FE
    /// </summary>
    public string? ReturnUrl { get; set; }
    
    /// <summary>
    /// Thông tin người tạo giao dịch
    /// </summary>
    public CustomerRequest? Customer { get; set; }
}