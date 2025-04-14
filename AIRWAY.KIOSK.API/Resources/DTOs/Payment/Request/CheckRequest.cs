namespace AIRWAY.KIOSK.API.Resources.DTOs.Payment.Request;

public sealed class CheckRequest
{
    /// <summary>
    /// Mã giao dịch
    /// </summary>
    public string? OrderCode { get; set; }
    
    /// <summary>
    /// Mã đơn hàng
    /// </summary>
    public string? BillId { get; set; }
    
    [JsonIgnore]
    public bool IsInternal { get; set; }
    
    [JsonIgnore]
    public bool UseNotify { get; set; }
}