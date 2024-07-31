namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.PaymentGateway.Request;

public sealed class CallbackRequest : DecryptRequest
{
    /// <summary>
    /// Mã đơn hàng
    /// </summary>
    [JsonPropertyName("orderCode")]
    public string? OrderCode { get; set; }

    /// <summary>
    /// Mã giao dịch thanh toán
    /// </summary>
    [JsonPropertyName("transCode")]
    public string? TransCode { get; set; }

    /// <summary>
    /// Phương thức thanh toán<br/>
    /// 0 - Chưa xác định (sẽ sử dụng khi thanh toán POS)<br/>
    /// 1 - Thanh toán qua sử dụng số dư Ví<br/>
    /// 2 - Thanh toán qua thẻ ATM và tài khoản ngân hàng<br/>
    /// 3 - Thanh toán qua thẻ tín dụng và ghi nợ khách hàng<br/>
    /// 4 - Thanh toán qua sử dụng ứng dụng Mobile Banking quét mã QR<br/>
    /// </summary>
    [JsonPropertyName("paymentMethod")]
    public int? PaymentMethod { get; set; }

    /// <summary>
    /// 0 - Chưa thanh toán
    /// 1 - Đã thanh toán thành công
    /// 2 - Đã thanh toán thất bại
    /// 3 - Đang chờ xử lý
    /// 4 - Hủy bởi người sử dụng
    /// 5 - Giao dịch đang chờ xử lý phía đối tác
    /// </summary>
    [JsonPropertyName("transStatus")]
    public int? TransStatus { get; set; }
    
    /// <summary>
    /// Token thanh toán (Trong trường hợp thanh toán có khởi tạo token thành công)
    /// </summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }
    
    /// <summary>
    /// Thông tin số thẻ được dùng để thanh toán tạo token
    /// </summary>
    [JsonPropertyName("cardNumber")]
    public string? CardNumber { get; set; }
}
