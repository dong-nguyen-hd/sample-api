using EPAY.AIRWAY.KIOSK.API.Domain.Models.Base;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Models;

/// <summary>
/// Thông tin dịch vụ bổ sung
/// </summary>
public sealed class AdditionalService : BaseModel
{
    public string Id { get; set; } = RelateText.GenId();
    
    public MyEnum.AdditionalServiceType Type { get; set; }

    /// <summary>
    /// Điểm khởi hành
    /// </summary>
    public string? StartPoint { get; set; }
    
    /// <summary>
    /// Điểm kết thúc
    /// </summary>
    public string? EndPoint { get; set; }
    
    /// <summary>
    /// Mã dịch vụ
    /// </summary>
    public string? Code { get; set; }
    
    /// <summary>
    /// Đơn vị tiền tệ
    /// </summary>
    public string? Currency { get; set; }
    
    /// <summary>
    /// Tên dịch vụ
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Giá
    /// </summary>
    public string? Price { get; set; }
    
    /// <summary>
    /// Giá trị
    /// </summary>
    public string? Value { get; set; }

    public string PassengerId { get; set; }
    public Passenger Passenger { get; set; } = null!;
}