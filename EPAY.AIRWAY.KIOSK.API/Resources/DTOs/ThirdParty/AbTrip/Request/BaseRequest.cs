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
    
    [JsonPropertyName("ListFareData")]
    public List<FareDataRequest>? ListFareData { get; set; }
}

public class FareDataRequest
{
    [JsonPropertyName("Session")]
    public string? Session { get; set; }

    [JsonPropertyName("FareDataId")]
    public int? FareDataId { get; set; }

    [JsonPropertyName("ListFlight")]
    public List<FlightRequest>? ListFlight { get; set; }
}

public class FlightRequest
{
    [JsonPropertyName("FlightValue")]
    public string? FlightValue { get; set; }
}