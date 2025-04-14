using AIRWAY.KIOSK.API.Domain.Models.Base;

namespace AIRWAY.KIOSK.API.Domain.Models;

/// <summary>
/// Thông tin người đặt vé
/// </summary>
public sealed class Contact : BaseModel
{
    public string Id { get; set; } = RelateText.GenId();
    
    /// <summary>
    /// Họ
    /// </summary>
    public string FirstName { get; set; } = null!;
    
    /// <summary>
    /// Tên
    /// </summary>
    public string LastName { get; set; } = null!;
    
    /// <summary>
    /// Giới tính <br/>
    /// true - nam <br/>
    /// false - nữ <br/>
    /// </summary>
    public bool Gender { get; set; }
    
    /// <summary>
    /// Số điện thoại
    /// </summary>
    public string? Phone { get; set; }
    
    /// <summary>
    /// Email
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Ngày sinh
    /// </summary>
    public DateOnly? BirthDay { get; set; }
    
    public string BillId { get; set; }
    public Bill Bill { get; set; } = null!;
}