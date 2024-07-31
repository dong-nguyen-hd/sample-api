using Models.ToJson;

namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Account.Request;

public sealed class CreateRequest
{
    /// <summary>
    /// Tên tài khoản
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Mật khẩu
    /// </summary>
    [SensitiveData]
    public string Password { get; set; }
    
    /// <summary>
    /// Tên người dùng
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Email
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Danh sách quyền truy cập
    /// </summary>
    public List<string> SystemRoles { get; set; }
    
    /// <summary>
    /// Thông tin bổ sung
    /// </summary>
    public AdditionData? AdditionData { get; set; }
}
