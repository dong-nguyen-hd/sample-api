namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.CustomHttpClient.Request;

public sealed class MyHttpRequest
{
    public Uri Uri { get; set; }
    public string? Payload { get; set; }
    public Enums.MyHttpMethod MyHttpMethod { get; set; }
    public int NumberRetry { get; set; }
    public bool EnableVerifyTls { get; set; }
    public List<HeaderRequest>? Headers { get; set; }
}