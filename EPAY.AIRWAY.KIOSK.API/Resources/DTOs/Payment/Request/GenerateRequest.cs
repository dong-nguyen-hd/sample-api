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
    public string BillId
    {
        get => _billId;
        set => _billId = value.ToLowerAndRemoveSpace();
    }

    private string _billId;

    public string? PosSerial { get; set; }

    public string? PosRefId { get; set; }

    public string? PosMerchantId { get; set; }

    public string? PosClientId { get; set; }

    public string? PosMerchantOutletId { get; set; }

    public string? PosTerminalId { get; set; }

    /// <summary>
    /// Tổng số tiền thanh toán
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Kênh bán hàng, được hiểu là nền tảng tạo giao dịch đơn hàng
    /// </summary>
    public PlatformType PlatformType { get; set; }
    
    public string? ReturnUrl { get; set; }
    public string? IdNumber { get; set; }
}