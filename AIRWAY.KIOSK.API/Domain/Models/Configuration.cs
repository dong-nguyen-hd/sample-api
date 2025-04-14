using AIRWAY.KIOSK.API.Domain.Models.Base;

namespace AIRWAY.KIOSK.API.Domain.Models;

/// <summary>
/// Thông tin cấu hình hệ thống
/// </summary>
public sealed class Configuration : BaseModel
{
    public string Id { get; set; } = RelateText.GenId();
    
    /// <summary>
    /// Mã cấu hình
    /// </summary>
    public string Key { get; set; }
    
    /// <summary>
    /// Giá trị cấu hình
    /// </summary>
    public string? Value { get; set; }
    
    /// <summary>
    /// Cấu hình sử dụng trong hệ thống BE <br/>
    /// true - cấu hình chỉ sử dụng trong hệ thống BE
    /// false - cấu hình có thể dùng bên ngoài hệ thống BE
    /// </summary>
    public bool Internal { get; set; }
}