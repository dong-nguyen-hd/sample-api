namespace AIRWAY.KIOSK.API.Domain.Models.ToJson;

public sealed record PaymentMethod
{
    public MyEnum.PaymentType Type { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
}
