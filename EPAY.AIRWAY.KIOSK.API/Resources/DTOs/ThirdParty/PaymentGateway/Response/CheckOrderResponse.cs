using EPAY.AIRWAY.KIOSK.API.Resources.Enums;

namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.PaymentGateway.Response;

public sealed class CheckOrderResponse : DecryptResponse
{
    /// <summary>
    /// Danh sách giao dịch được tìm thấy
    /// </summary>
    [JsonPropertyName("transactionInfos")]
    public List<CheckOrderInner>? TransactionInfos { get; set; }

    /// <summary>
    /// Trạng thái thanh toán được mapping tương ứng BE
    /// </summary>
    [JsonIgnore]
    public MappingPaymentGatewayToSystem? MappingFromPaymentGateway { get; set; }
}

public sealed class CheckOrderInner
{
    /// <summary>
    /// Mã đơn hàng do Merchant tạo
    /// </summary>
    [JsonPropertyName("orderCode")]
    public string? OrderCode { get; set; }

    /// <summary>
    /// Mã đơn hàng do Merchant tạo
    /// </summary>
    [JsonPropertyName("transCode")]
    public string? TransCode { get; set; }

    /// <summary>
    /// Mã hóa đơn cho Merchant tạo nếu cần
    /// </summary>
    [JsonPropertyName("billId")]
    public string? BillId { get; set; }

    /// <summary>
    /// 1: Ngay <br/>
    /// 2: Tạm giữ<br/>
    /// </summary>
    [JsonPropertyName("paymentType")]
    public int? PaymentType { get; set; }

    /// <summary>
    /// Số tiền cần thanh toán
    /// </summary>
    [JsonPropertyName("totalAmount")]
    public long? TotalAmount { get; set; }

    /// <summary>
    /// Tiền đơn hàng
    /// </summary>
    [JsonPropertyName("orderAmount")]
    public long? OrderAmount { get; set; }

    /// <summary>
    /// Số tiền phí thu hộ Merchant
    /// </summary>
    [JsonPropertyName("feeAmount")]
    public long? FeeAmount { get; set; }

    /// <summary>
    /// Thời gian thanh toán. Thời gian unix theo milisecond
    /// </summary>
    [JsonPropertyName("paymentTime")]
    public long? PaymentTime { get; set; }

    /// <summary>
    /// Mô tả đơn hàng
    /// </summary>
    [JsonPropertyName("orderDescription")]
    public string? OrderDescription { get; set; }

    /// <summary>
    /// Loại tiền thanh toán (mặc định VND)
    /// </summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    /// <summary>
    /// Thời gian cho phép thanh toán; tính theo phút, mặc định = 24 giờ (1440 phút)
    /// </summary>
    [JsonPropertyName("timeLimit")]
    public long? TimeLimit { get; set; }

    /// <summary>
    /// BankCode/walletCode khách dùng để thanh toán, trường hợp chưa gửi sang đối tác thanh toán thì giá trị là rỗng
    /// </summary>
    [JsonPropertyName("partnerCode")]
    public string? PartnerCode { get; set; }

    /// <summary>
    /// Phương thức thanh toán: <br/>
    /// 1 - Thanh toán qua sử dụng số dư Ví <br/>
    /// 2 – Thanh toán qua thẻ ATM và tài khoản ngân hàng <br/>
    /// 3 - Thanh toán qua thẻ tín dụng và ghi nợ khách hàng <br/>
    /// 4 - Thanh toán qua sử dụng ứng dụng Mobile Banking quét mã QR <br/>
    /// </summary>
    [JsonPropertyName("paymentMethod")]
    public int? PaymentMethod { get; set; }

    /// <summary>
    /// Thời gian cho phép thanh toán; tính theo phút, mặc định = 24 giờ (1440 phút) <br/>
    /// 0 - Chưa thanh toán <br/>
    /// 1 -  Đã thanh toán thành công <br/>
    /// 2 - Đã thanh toán thất bại <br/>
    /// 3 -  Đang chờ xử lý <br/>
    /// 4 - Hủy bởi người sử dụng <br/>
    /// 5 - Giao dịch đang chờ xử lý phía đối tác <br/>
    /// </summary>
    [JsonPropertyName("transStatus")]
    public int? TransStatus { get; set; }
}

/// <summary>
/// Dữ liệu chuyển đổi từ cổng thanh toán về hệ thống BE
/// </summary>
public sealed class MappingPaymentGatewayToSystem
{
    public PaymentStatus PaymentStatus { get; set; }
    public string? PartnerPaymentType { get; set; }
    public string? TransCode { get; set; }
}