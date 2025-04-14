namespace AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.PaymentGateway.Response;

public sealed class CreateOrderResponse : DecryptResponse
{
    /// <summary>
    /// Url để chuyển qua trang thanh toán của DongND
    /// </summary>
    [JsonPropertyName("paymentUrl")]
    public string? PaymentUrl { get; set; }

    /// <summary>
    /// Chuỗi QR khi sử dụng phương thức thanh toán 04 - Thanh toán qua sử dụng ứng dụng Mobile Banking quét mã QR
    /// </summary>
    [JsonPropertyName("qrCode")]
    public string? QrCode { get; set; }

    /// <summary>
    /// Thời gian cho phép thanh toán; tính theo phút, mặc định = 24 giờ (1440 phút)
    /// </summary>
    [JsonPropertyName("timeLimit")]
    public int? TimeLimit { get; set; }

    /// <summary>
    /// Thời gian hiệu lực của QR; tính theo phút, mặc định = 24 giờ (1440 phút)
    /// </summary>
    [JsonPropertyName("qrExpire")]
    public int? QrExpire { get; set; }
    
    /// <summary>
    /// deepLink mở app<br/>
    /// Vd : xxxx://xxxxxx
    /// </summary>
    [JsonPropertyName("deepLink")]
    public string? DeepLink { get; set; }
}