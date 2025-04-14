namespace AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;

public sealed class InvoiceResponse
{
    /// <summary>
    /// Mã số thuế
    /// </summary>
    public string? TaxCode { get; set; }

    /// <summary>
    /// Tên doanh nghiệp
    /// </summary>
    public string? CompanyNameReceive { get; set; }

    /// <summary>
    /// Địa chỉ người nhận
    /// </summary>
    public string? AddressReceive { get; set; }

    /// <summary>
    /// Tên thành phố (người nhận)
    /// </summary>
    public string? CityNameReceive { get; set; }

    /// <summary>
    /// Tên người đại diện
    /// </summary>
    public string? ReceiverReceive { get; set; }
}