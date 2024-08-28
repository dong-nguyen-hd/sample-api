using EPAY.AIRWAY.KIOSK.API.Domain.Models.Base;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Models;

public class Device : BaseModel
{
    public string Id { get; set; } = RelateText.GenId();

    public string Code { get; set; } = null!;
    
    public string? PosSerial { get; set; }

    public string? PosRefId { get; set; }

    public string? PosMerchantId { get; set; }

    public string? PosClientId { get; set; }

    public string? PosMerchantOutletId { get; set; }

    public string? PosTerminalId { get; set; }
}