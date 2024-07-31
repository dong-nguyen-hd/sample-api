namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.PaymentGateway.Response;

public sealed class RefundResponse : DecryptResponse
{
    /// <summary>
    /// Mã thanh toán (là trường transCode trong bản tin trả kết quả cho merchant – tham khảo mục 4.4, 4.5)
    /// </summary>
    [JsonPropertyName("referenceId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ReferenceId { get; set; }

    /// <summary>
    /// Mã đơn hàng
    /// </summary>
    [JsonPropertyName("orderCode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? OrderCode { get; set; }

    /// <summary>
    /// Mã giao dịch hoàn tiền
    /// </summary>
    [JsonPropertyName("transCode")]
    public string? TransCode { get; set; }

    /// <summary>
    /// Phương thức thanh toán <br/>
    /// 1 - Thanh toán qua sử dụng số dư Ví <br/>
    /// 2 – Thanh toán qua thẻ ATM và tài khoản ngân hàng <br/>
    /// 3 - Thanh toán qua thẻ tín dụng và ghi nợ khách hàng <br/>
    /// 4 - Thanh toán qua sử dụng ứng dụng Mobile Banking quét mã QR <br/>
    /// </summary>
    [JsonPropertyName("paymentMethod")]
    public int? PaymentMethod { get; set; }

    /// <summary>
    /// 1 - Đã thanh toán thành công
    /// 2 - Đã thanh toán thất bại
    /// 3 - Chờ xử lý
    /// </summary>
    [JsonPropertyName("transStatus")]
    public string? TransStatus { get; set; }

    /// <summary>
    /// Hình thức refund:
    /// 0: Hoàn tiền toàn phần
    /// 1: Hoàn tiền từng phần
    /// </summary>
    [JsonPropertyName("refundType")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? RefundType { get; set; }

    /// <summary>
    /// Số tiền refund, với hoàn tiền toàn phần thì số tiền refund phải bằng số tiền đã thanh toán
    /// </summary>
    [JsonPropertyName("amount")]
    public long Amount { get; set; }

    /// <summary>
    /// Thời gian hoàn tiền unix theo milisecond
    /// </summary>
    [JsonPropertyName("refundTime")]
    public long RefundTime { get; set; }
}