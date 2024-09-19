namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;

public sealed class ContactResponse
{
    /// <summary>
    /// Họ người liên hệ
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Tên người liên hệ
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Danh xưng: <br/>
    /// true - ông <br/>
    /// false - bà
    /// </summary>
    public bool Gender { get; set; }

    /// <summary>
    ///  Số điện thoại
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    public string? Email { get; set; }
}