namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.PaymentGateway.Request;

public sealed class CreateOrderRequest : DecryptRequest
{
    /// <summary>
    /// Được Epay cung cấp (Được băm với SHA256)
    /// </summary>
    [JsonPropertyName("merchantPassword")]
    public string? MerchantPassword { get; set; }

    /// <summary>
    /// Mã đơn hàng do Merchant tạo
    /// </summary>
    [JsonPropertyName("orderCode")]
    public string? OrderCode { get; set; }

    /// <summary>
    /// Mã hóa đơn do Merchant tạo nếu cần
    /// </summary>
    [JsonPropertyName("billId")]
    public string? BillId { get; set; }

    /// <summary>
    /// 1: Ngay, 2: Tạm giữ
    /// </summary>
    [JsonPropertyName("paymentType")]
    public int PaymentType { get; set; }

    /// <summary>
    /// Mã ngành nghề kinh doanh đặc thù (mặc định bằng 0000 với các thanh toán thông thường)
    /// </summary>
    [JsonPropertyName("businessType")]
    public string? BusinessType { get; set; } = "0000";

    /// <summary>
    /// Số tiền cần thanh toán
    /// </summary>
    [JsonPropertyName("totalAmount")]
    public long TotalAmount { get; set; }

    /// <summary>
    /// Tiền đơn hàng
    /// </summary>
    [JsonPropertyName("orderAmount")]
    public long OrderAmount { get; set; }

    /// <summary>
    /// Tiền phí thu hộ Merchant
    /// </summary>
    [JsonPropertyName("feeAmount")]
    public long FeeAmount { get; set; }

    /// <summary>
    /// Mô tả đơn hàng
    /// </summary>
    [JsonPropertyName("orderDescription")]
    public string? OrderDescription { get; set; }

    /// <summary>
    /// Loại tiền thanh toán (mặc định VND)
    /// </summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; } = "VND";

    /// <summary>
    /// Địa chỉ website nhận thông báo giao dịch "Thành công/thất bại"
    /// </summary>
    [JsonPropertyName("returnUrl")]
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// Địa chỉ website nhận "Hủy giao dịch"
    /// </summary>
    [JsonPropertyName("cancelUrl")]
    public string? CancelUrl { get; set; }

    /// <summary>
    /// Link trang Merchant trước khi chuyển sang trang EPAY
    /// </summary>
    [JsonPropertyName("againUrl")]
    public string? AgainUrl { get; set; }

    /// <summary>
    /// Thời gian cho phép thanh toán; tính theo phút, mặc định = 24 giờ (1440 phút)
    /// </summary>
    [JsonPropertyName("timeLimit")]
    public int TimeLimit { get; set; }

    /// <summary>
    /// Tên người mua
    /// </summary>
    [JsonPropertyName("customerFullName")]
    public string? CustomerFullName { get; set; }

    /// <summary>
    /// Địa chỉ Email người mua
    /// </summary>
    [JsonPropertyName("customerEmail")]
    public string? CustomerEmail { get; set; }

    /// <summary>
    /// Điện thoại người mua
    /// </summary>
    [JsonPropertyName("customerMobile")]
    public string? CustomerMobile { get; set; }

    /// <summary>
    /// Địa chỉ người mua hàng
    /// </summary>
    [JsonPropertyName("customerAddress")]
    public string? CustomerAddress { get; set; }
    
    /// <summary>
    /// Thông tin người thanh toán <br/>
    /// (Áp dụng khi sử dụng chức năng SSO qua ví EPAY, trường walletFunctionType = 1 và paymentMethod = 01)
    /// </summary>
    [JsonPropertyName("customerIdNumber")]
    public string? CustomerIdNumber { get; set; }

    /// <summary>
    /// sessionId
    /// </summary>
    [JsonPropertyName("sessionId")]
    [JsonIgnore]
    public string? SessionId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonPropertyName("lockTime")]
    [JsonIgnore]
    public long LockTime { get; set; } // lockTime

    [JsonPropertyName("lockInterval")]
    [JsonIgnore]
    public int LockInterval { get; set; } // lockTime

    /// <summary>
    /// Tổng số sản phẩm trong đơn hàng
    /// </summary>
    [JsonPropertyName("totalGoods")]
    public int TotalGoods { get; set; }

    /// <summary>
    /// Danh sách các mặt hàng
    /// </summary>
    [JsonPropertyName("detailGoods")]
    public List<DetailInfo>? DetailGoods { get; set; }

    /// <summary>
    /// Đại lý
    /// </summary>
    [JsonPropertyName("agencyCode")]
    public string? AgencyCode { get; set; }
    
    /// <summary>
    /// Tên đại lý bán hàng
    /// </summary>
    [JsonPropertyName("agencyName")]
    public string? AgencyName { get; set; }
    
    /// <summary>
    /// Tên nhà cung cấp dịch vụ
    /// </summary>
    [JsonPropertyName("provider")]
    public string? Provider { get; set; }

    /// <summary>
    /// Thông tin bổ sung
    /// </summary>
    [JsonPropertyName("addInfo")]
    public string? AddInfo { get; set; }

    /// <summary>
    /// Mã khách hang của Merchant
    /// </summary>
    [JsonPropertyName("customerCode")]
    public string? CustomerCode { get; set; }

    /// <summary>
    /// Token thông tin thanh toán trước đó <br/>
    /// (Chỉ sử dụng khi thanh toán dùng token)
    /// </summary>
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    /// <summary>
    /// Kênh thanh toán:<br/>
    /// 01 - Website<br/>
    /// 02 - Mobile App<br/>
    /// 03 - POS<br/>
    /// 04 - SmartPOS<br/>
    /// 05 - QR tĩnh ĐVCNTT<br/>
    /// 06 - Kiosk<br/>
    /// 07 - Mini Kiosk<br/>
    /// 08 - SmartGate<br/>
    /// </summary>
    [JsonPropertyName("channelCode")]
    public string? ChannelCode { get; set; }

    /// <summary>
    /// Phương thức thanh toán:<br/>
    /// 00 - Chưa xác định(sẽ sử dụng khi thanh toán POS)<br/>
    /// 01 - Thanh toán qua sử dụng số dư Ví<br/>
    /// 02 - Thanh toán qua thẻ ATM và tài khoản ngân hàng<br/>
    /// 03 - Thanh toán qua thẻ tín dụng và ghi nợ khách hàng<br/>
    /// 04 - Thanh toán qua sử dụng ứng dụng Mobile Banking quét mã QR<br/>
    /// </summary>
    [JsonPropertyName("paymentMethod")]
    public string? PaymentMethod { get; set; }

    /// <summary>
    /// Loại thẻ hay tài khoản (Chỉ sử dụng khi paymentMethod là 02):<br/>
    /// Card - Thẻ<br/>
    /// Account - Tài khoản
    /// </summary>
    [JsonPropertyName("typeCardAccount")]
    public string? TypeCardAccount { get; set; }

    /// <summary>
    /// Mã Kiosk
    /// </summary>
    [JsonPropertyName("kioskId")]
    public string? KioskId { get; set; }

    /// <summary>
    /// Số serial của POS (Chỉ sử dụng khi thanh toán POS)
    /// </summary>
    [JsonPropertyName("posSerial")]
    public string? PosSerial { get; set; }

    /// <summary>
    /// RefId của hệ thống EDCC khi thanh toán qua POS (Chỉ sử dụng khi thanh toán POS)
    /// </summary>
    [JsonPropertyName("posRefId")]
    public string? PosRefId { get; set; }

    /// <summary>
    /// MerchantId của hệ thống EDCC khi thanh toán qua POS (Chỉ sử dụng khi thanh toán POS)
    /// </summary>
    [JsonPropertyName("posMerchantId")]
    public string? PosMerchantId { get; set; }

    /// <summary>
    /// ClientId của hệ thống EDCC khi thanh toán qua POS (Chỉ sử dụng khi thanh toán POS)
    /// </summary>
    [JsonPropertyName("posClientId")]
    public string? PosClientId { get; set; }

    /// <summary>
    /// MerchantOutletId của hệ thống EDCC khi thanh toán qua POS (Chỉ sử dụng khi thanh toán POS)
    /// </summary>
    [JsonPropertyName("posMerchantOutletId")]
    public string? PosMerchantOutletId { get; set; }

    /// <summary>
    /// TerminalId của hệ thống EDCC khi thanh toán qua POS (Chỉ sử dụng khi thanh toán POS)
    /// </summary>
    [JsonPropertyName("posTerminalId")]
    public string? PosTerminalId { get; set; }

    /// <summary>
    /// Có khởi tạo token không (Chỉ sử dụng khi thanh toán cần khởi tạo token)
    /// </summary>
    [JsonPropertyName("saveToken")]
    public bool? SaveToken { get; set; }

    /// <summary>
    /// Sử dụng khi payment-method là 01 <br/>
    /// 0: Thanh toán ví trên hostedform của CTT <br/>
    /// 1: Thanh toán ví sử dụng web ví (Áp dụng khi sử dụng chức năng SSO qua ví EPAY) <br/>
    /// 2: Sử dụng deeplink của ví <br/>
    /// </summary>
    [JsonPropertyName("walletFunctionType")]
    public int? WalletFunctionType { get; set; }
}

public sealed class DetailInfo
{
    /// <summary>
    /// Tên mặt hàng
    /// </summary>
    [JsonPropertyName("goodsName")]
    public string? GoodsName { get; set; }

    /// <summary>
    /// Số lượng mặt hàng
    /// </summary>
    [JsonPropertyName("goodsQuantity")]
    public int? GoodsQuantity { get; set; }

    /// <summary>
    /// Đơn giá
    /// </summary>
    [JsonPropertyName("goodsPrice")]
    public long? GoodsPrice { get; set; }

    /// <summary>
    /// Đường link mặt hàng
    /// </summary>
    [JsonPropertyName("goodsUrl")]
    public string? GoodsUrl { get; set; }

    /// <summary>
    /// Mã mặt hàng
    /// </summary>
    [JsonPropertyName("goodsCode")]
    public string? GoodsCode { get; set; }
}