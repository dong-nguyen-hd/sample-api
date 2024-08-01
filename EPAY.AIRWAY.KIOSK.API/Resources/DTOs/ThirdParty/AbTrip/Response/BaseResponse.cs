namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip.Response;

public abstract class BaseResponse
{
    /// <summary>
    /// Kết quả kết nối
    /// </summary>
    [JsonPropertyName("Status")]
    public bool? Status { get; set; }

    /// <summary>
    /// Mã lỗi (nếu có)
    /// </summary>
    [JsonPropertyName("ErrorCode")]
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Thông báo lỗi (nếu có)
    /// </summary>
    [JsonPropertyName("Message")]
    public string? Message { get; set; }
}