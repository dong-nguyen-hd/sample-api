namespace AIRWAY.KIOSK.API.Resources.DTOs.Device.Request;

public class CreateRequest
{
    public Guid? LocationId { get; set; }
    
    public Guid? ServicePartnerId { get; set; }
    
    public string? PosSerial { get; set; }

    public string? PosRefId { get; set; }

    public string? PosMerchantId { get; set; }

    public string? PosClientId { get; set; }

    public string? PosMerchantOutletId { get; set; }

    public string? PosTerminalId { get; set; }
}