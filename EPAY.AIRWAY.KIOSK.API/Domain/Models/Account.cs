using EPAY.AIRWAY.KIOSK.API.Domain.Models.Base;
using EPAY.AIRWAY.KIOSK.API.Domain.Models.ToJson;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Models;

/// <summary>
/// Thông tin về tài khoản
/// </summary>
public sealed class Account : BaseModel
{
    public string Id { get; set; } = RelateText.GenId();
    
    /// <summary>
    /// Tên đăng nhập
    /// </summary>
    public string UserName { get; set; }
    
    /// <summary>
    /// Mật khẩu
    /// </summary>
    public string Password { get; set; }
    
    /// <summary>
    /// Tên
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Ảnh đại diện
    /// </summary>
    public string? Avatar { get; set; }
    
    /// <summary>
    /// Email
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Xác định tài khoản có phải sử dụng mã xác thực opt hay không
    /// </summary>
    // public bool? HasOtp { get; set; }
    
    /// <summary>
    /// Danh sách role
    /// </summary>
    public List<string> SystemRoles { get; set; }
    
    public AdditionData? AdditionData { get; set; }
    public HashSet<RefreshToken>? RefreshTokens { get; set; }
}
