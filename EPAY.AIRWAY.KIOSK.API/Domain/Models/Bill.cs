using EPAY.AIRWAY.KIOSK.API.Domain.Models.Base;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Models;

public sealed class Bill : BaseModel
{
    public string Id { get; set; } = RelateText.GenId();

    public Invoice? Invoice { get; set; }
    public Contact? Contact { get; set; }
    public HashSet<Model.Reservation>? Reservations { get; set; }
    public HashSet<Model.Passenger>? Tickets { get; set; }
    public HashSet<Model.PaymentTransaction>? PaymentTransactions { get; set; }
}