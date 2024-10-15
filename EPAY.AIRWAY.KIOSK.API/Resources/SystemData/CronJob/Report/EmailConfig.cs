namespace EPAY.AIRWAY.KIOSK.API.Resources.SystemData.CronJob.Report;

public sealed record EmailConfig
{
    /// <summary>
    /// Địa chỉ nhận <br/>
    /// Dữ liệu với nhiều email phân cách bằng đấu chấm phẩy ';' ví dụ: "example@email.com;example1@email.com"
    /// </summary>
    public string? AddressTo { get; set; }

    /// <summary>
    /// Địa chỉ gửi
    /// </summary>
    public string? AddressFrom { get; set; }

    /// <summary>
    /// Địa chỉ CC <br/>
    /// Dữ liệu với nhiều email phân cách bằng đấu chấm phẩy ';' ví dụ: "example@email.com;example1@email.com"
    /// </summary>
    public string? AddressCC { get; set; }

    /// <summary>
    /// Địa chỉ BCC <br/>
    /// Dữ liệu với nhiều email phân cách bằng đấu chấm phẩy ';' ví dụ: "example@email.com;example1@email.com"
    /// </summary>
    public string? AddressBCC { get; set; }

    /// <summary>
    /// Mật khẩu địa chỉ gửi
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Dữ liệu cấu hình server email với cấu trúc [host]:[port]
    /// </summary>
    public string? Host { get; set; }

    /// <summary>
    /// Tiêu đề email
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// Nội dung email
    /// </summary>
    public string? Body { get; set; }

    /// <summary>
    /// Tên tệp đính kèm
    /// </summary>
    public string? FileName { get; set; }
}