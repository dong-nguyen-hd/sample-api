namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.PaymentGateway.Request;

public sealed class RefundRequest : DecryptRequest
{
    /// <summary>
    /// Mã thanh toán (là trường transCode trong bản tin trả kết quả cho merchant – tham khảo mục 4.4, 4.5) <br/>
    /// [Bổ sung trao đổi với Tuấn Đào] transCode có thể lấy trong api "check_status"
    /// </summary>
    [JsonPropertyName("referenceId")]
    public string? ReferenceId { get; set; }

    /// <summary>
    /// Mã đơn hàng
    /// </summary>
    [JsonPropertyName("orderCode")]
    public string? OrderCode { get; set; }

    /// <summary>
    /// Hình thức refund:
    /// 0: Hoàn tiền toàn phần
    /// 1: Hoàn tiền từng phần
    /// </summary>
    [JsonPropertyName("refundType")]
    public int? RefundType { get; set; }

    /// <summary>
    /// Số tiền refund, với hoàn tiền toàn phần thì số tiền refund phải bằng số tiền đã thanh toán
    /// </summary>
    [JsonPropertyName("amount")]
    public long? Amount { get; set; }

    /// <summary>
    /// Thời gian hoàn tiền unix theo milisecond
    /// </summary>
    [JsonPropertyName("refundTime")]
    public long? RefundTime { get; set; }
}