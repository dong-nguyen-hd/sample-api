namespace AIRWAY.KIOSK.API.Resources.Enums;

public enum PaymentStatus : byte
{
    /// <summary>
    /// Đã thanh toán thất bại
    /// </summary>
    Fail = 0,
    
    /// <summary>
    /// Đã thanh toán thành công
    /// </summary>
    Success = 1,
    
    /// <summary>
    /// Chờ xử lí
    /// </summary>
    Pending = 2,
    
    /// <summary>
    /// Thanh toán bị huỷ
    /// </summary>
    Cancel = 3,
    
    /// <summary>
    /// Khởi tạo giao dịch đã được đối tác phản hồi
    /// </summary>
    Init = 4,
    
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
}