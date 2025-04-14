using AIRWAY.KIOSK.API.Domain.Models.Base;

namespace AIRWAY.KIOSK.API.Domain.Models;

/// <summary>
/// Thông tin hoá đơn điện tử
/// </summary>
public sealed class Invoice : BaseModel
{
    public string Id { get; set; } = RelateText.GenId();
    
    /// <summary>
    /// Mã số thuế
    /// </summary>
    public string? TaxCode { get; set; }
    
    /// <summary>
    /// Tên doanh nghiệp
    /// </summary>
    public string? CompanyNameReceive { get; set; }
    
    /// <summary>
    /// Địa chỉ doanh nghiệp
    /// </summary>
    public string? AddressReceive { get; set; }
    
    /// <summary>
    /// Tên thành phố
    /// </summary>
    public string? CityNameReceive { get; set; }
    
    /// <summary>
    /// Tên người đại diện doanh nghiệp
    /// </summary>
    public string? ReceiverReceive { get; set; }

    public string BillId { get; set; }
    public Bill Bill { get; set; } = null!;
}