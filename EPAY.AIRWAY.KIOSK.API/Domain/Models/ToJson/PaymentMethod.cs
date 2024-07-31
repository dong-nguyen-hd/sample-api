namespace EPAY.AIRWAY.KIOSK.API.Domain.Models.ToJson;

using Resources.Enums;

public sealed class PaymentMethod
{
    public PaymentType Type { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
}
