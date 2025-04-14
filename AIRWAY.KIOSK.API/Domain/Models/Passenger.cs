using AIRWAY.KIOSK.API.Domain.Models.Base;

namespace AIRWAY.KIOSK.API.Domain.Models;

/// <summary>
/// Thông tin hành khách mua vé
/// </summary>
public sealed class Passenger : BaseModel
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
    /// true -  nam <br/>
    /// false - nữ <br/>
    /// </summary>
    public bool Gender { get; set; }
    
    /// <summary>
    /// Phân loại hành khách
    /// </summary>
    public MyEnum.PassengerType? Type { get; set; }
    
    /// <summary>
    /// Ngày sinh
    /// </summary>
    public DateOnly? BirthDay { get; set; }

    public string BillId { get; set; }
    public Bill Bill { get; set; } = null!;
    public HashSet<Model.AdditionalService>? AdditionalServices { get; set; }
}