namespace AIRWAY.KIOSK.API.Resources.DTOs.CustomHttpClient.Request;

public sealed class MyHttpRequest<TReq>
{
    /// <summary>
    /// Đường dẫn thực hiện request
    /// </summary>
    public Uri? Uri { get; set; }
    
    /// <summary>
    /// Body-payload thực hiện request
    /// </summary>
    public TReq? Payload { get; set; }
    
    /// <summary>
    /// Http-method thực hiện request
    /// </summary>
    public Enums.MyHttpMethod MyHttpMethod { get; set; }
    
    /// <summary>
    /// Số lần thử lại request<br/>
    /// Với mặc định = 0, chỉ request 1 lần và không thử lại
    /// Với > 0, tương ứng với số lần thử lại.
    /// </summary>
    public int NumberRetry { get; set; }
    
    /// <summary>
    /// Bật/tắt xác thực Tls
    /// </summary>
    public bool EnableVerifyTls { get; set; }
    
    /// <summary>
    /// Header của request
    /// </summary>
    public List<HeaderRequest>? Headers { get; set; }
}