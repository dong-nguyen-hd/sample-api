namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;

public sealed class PassengerResponse
{
    /// <summary>
    /// Họ
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Tên đệm và tên
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Ngày sinh
    /// </summary>
    public DateTime? Birthday { get; set; }

    /// <summary>
    /// Giới tính: <br/>
    /// true – nam <br/>
    /// false – nữ <br/>
    /// </summary>
    public bool? Gender { get; set; }
    
    /// <summary>
    /// Phân loại hành khách
    /// </summary>
    public MyEnum.PassengerType Type { get; set; }

    /// <summary>
    /// Danh sách hành lý ký gửi
    /// </summary>
    public List<BaggageResponse>? ListBaggage { get; set; }

    /// <summary>
    /// Danh sách dịch vụ bổ sung
    /// </summary>
    public List<AncillaryResponse>? ListService { get; set; }
}