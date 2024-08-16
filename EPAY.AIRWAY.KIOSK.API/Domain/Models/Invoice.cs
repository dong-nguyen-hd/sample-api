using EPAY.AIRWAY.KIOSK.API.Domain.Models.Base;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Models;

public sealed class Invoice : BaseModel
{
    public string Id { get; set; } = RelateText.GenId();
    public string? TaxCode { get; set; }
    public string? CompanyNameReceive { get; set; }
    public string? AddressReceive { get; set; }
    public string? CityNameReceive { get; set; }
    public string? ReceiverReceive { get; set; }

    public string BillId { get; set; }
    public Bill Bill { get; set; } = null!;
}