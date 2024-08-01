using EPAY.AIRWAY.KIOSK.API.Domain.Models.Base;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Models;

public sealed class Bill : BaseModel
{
    public int Id { get; set; }
    public HashSet<Model.Ticket>? Tickets { get; set; }
    public HashSet<Model.PaymentTransaction>? PaymentTransactions { get; set; }
}