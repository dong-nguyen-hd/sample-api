namespace AIRWAY.KIOSK.API.Resources.Enums;

public enum ServiceStatus : byte
{
    /// <summary>
    /// Xuất vé thất bại
    /// </summary>
    Fail = 0,
    
    /// <summary>
    /// Xuất vé thành công
    /// </summary>
    Success = 1,
    
    /// <summary>
    /// Khởi tạo giao dịch nhưng chưa thực hiện request tới đối tác
    /// </summary>
    None = 5,
    
    /// <summary>
    /// Không nhận được phản hồi trạng thái từ đối tác
    /// </summary>
    Timeout = 6,
    
    /// <summary>
    /// Lỗi không xác định
    /// </summary>
    Unknown = 7,
    
    /// <summary>
    /// Không xuất được toàn bộ vé
    /// </summary>
    HalfSuccess = 8
}