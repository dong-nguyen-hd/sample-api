namespace EPAY.AIRWAY.KIOSK.API.Domain.Models.ToJson;

public sealed class PaymentMethod
{
    public MyEnum.PaymentType Type { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
}
