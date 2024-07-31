namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip.Request;

public abstract class BaseRequest
{
    /// <summary>
    /// Tài khoản
    /// </summary>
    [JsonPropertyName("Username")]
    public string? Username { get; set; }
    
    /// <summary>
    /// Mật khẩu
    /// </summary>
    [JsonPropertyName("Password")]
    public string? Password { get; set; }
}