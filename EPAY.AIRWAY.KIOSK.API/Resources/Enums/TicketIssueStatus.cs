namespace EPAY.AIRWAY.KIOSK.API.Resources.Enums;

public enum TicketIssueStatus : byte
{
    /// <summary>
    /// Hiển thị kết quả thanh toán thất bại, điều kiện: <br/>
    /// 1) Chưa xác định thanh toán thành oông <br/>
    /// 2) Chưa xác định xuất vé thành công <br/>
    /// </summary>
    Fail = 0,
    
    /// <summary>
    /// Hiển thị kết quả thanh toán thành công, điều kiện: <br/>
    /// 1) Đã thanh toán thành công <br/>
    /// 2) Đã xuất vé thành công <br/>
    /// </summary>
    Success = 1,
    
    /// <summary>
    /// Hiển thị kết quả thanh toán thành công, điều kiện: <br/>
    /// 1) Đã thanh toán thành công <br/>
    /// 2) Xuất vé không thành công toàn bộ <br/>
    /// </summary>
    HalfSuccess = 3,
}