namespace EPAY.AIRWAY.KIOSK.API.Resources.Enums;

public enum TicketIssueStatus : byte
{
    /// <summary>
    /// Hiển thị kết quả thanh toán thất bại, điều kiện: <br/>
    /// 1) Chưa xác định thanh toán thành oông <br/>
    /// </summary>
    Fail = 0,
    
    /// <summary>
    /// Hiển thị kết quả thanh toán thành công, điều kiện: <br/>
    /// 1) Đã thanh toán thành công <br/>
    /// </summary>
    Success = 1
}