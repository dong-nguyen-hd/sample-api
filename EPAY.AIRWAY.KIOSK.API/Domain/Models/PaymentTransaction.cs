using EPAY.AIRWAY.KIOSK.API.Domain.Models.Base;
using EPAY.AIRWAY.KIOSK.API.Resources.Enums;
using IdGen;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Models;

public sealed class PaymentTransaction : BaseModel
{
    public uint Version { get; set; }

    public string Id { get; set; } = RelateText.GenId();

    public string? TraceId
    {
        get => _traceId;
        set => _traceId = value?.Replace(':', '_');
    }

    private string? _traceId;

    /// <summary>
    /// Phương thức thực hiện giao dịch
    /// </summary>
    public PaymentType PaymentType { get; set; }

    /// <summary>
    /// Unique - Mã định danh cho mỗi lần thực hiện gọi cổng thanh toán
    /// </summary>
    public string OrderCode
    {
        get => _orderCode;
        set => _orderCode = value.ToLowerAndRemoveSpace();
    }

    private string _orderCode;

    /// <summary>
    /// Số căn cước công dân của khách hàng tạo giao dịch
    /// </summary>
    public string? IdNumber { get; set; }

    /// <summary>
    /// Trạng thái thanh toán phía đối tác cung cấp dịch vụ
    /// </summary>
    public PaymentStatus ServiceProviderStatus { get; set; }

    /// <summary>
    /// Mã giao dịch phía cổng thanh toán trả về
    /// </summary>
    public string? TransCode { get; set; }

    /// <summary>
    /// Trạng thái của order-code phía cổng thanh toán
    /// </summary>
    public PaymentStatus PaymentProviderStatus { get; set; }

    /// <summary>
    /// Nội dung QR cổng thanh toán trả về
    /// </summary>
    public string? Qr { get; set; }

    public string? PaymentUrl { get; set; }
    public string? PaymentDeeplink { get; set; }
    public string? ReturnUrl { get; set; }

    public string? PosSerial { get; set; }

    public string? PosRefId { get; set; }

    public string? PosMerchantId { get; set; }

    public string? PosClientId { get; set; }

    public string? PosMerchantOutletId { get; set; }

    public string? PosTerminalId { get; set; }

    /// <summary>
    /// Mã thiết bị thực hiện gọi thanh toán
    /// </summary>
    public string? DeviceId { get; set; }

    /// <summary>
    /// Thời gian hết hạn giao dịch
    /// </summary>
    public DateTime? ExpiredDatetimeUtc { get; set; }

    /// <summary>
    /// Tổng số tiền thanh toán
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Kênh bán hàng, được hiểu là nền tảng tạo giao dịch đơn hàng
    /// </summary>
    public PlatformType PlatformType { get; set; }

    /// <summary>
    /// Mã phương thức thanh toán định nghĩa phía đối tác<br/>
    /// Tham chiếu giá trị tới "paymentMethod" của cổng thanh toán<br/>
    /// Phương thức thanh toán:<br/>
    /// 00 - Chưa xác định (sẽ sử dụng khi thanh toán POS)<br/>
    /// 01 - Thanh toán qua sử dụng số dư Ví<br/>
    /// 02 - Thanh toán qua thẻ ATM và tài khoản ngân hàng<br/>
    /// 03 - Thanh toán qua thẻ tín dụng và ghi nợ quốc tế<br/>
    /// 04 - Thanh toán qua sử dụng ứng dụng Mobile Banking quét mã QR<br/>
    /// </summary>
    public string? PartnerPaymentType { get; set; }

    /// <summary>
    /// Mã định danh cho mỗi đơn hàng
    /// </summary>
    public string BillId { get; set; }

    public Model.Bill Bill { get; set; }

    public HashSet<TransactionTracking>? TransactionTrackings { get; set; }
}