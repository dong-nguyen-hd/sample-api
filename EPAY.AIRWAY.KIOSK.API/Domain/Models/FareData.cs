using EPAY.AIRWAY.KIOSK.API.Domain.Models.Base;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Models;

public sealed class FareData : BaseModel
{
    public string Id { get; set; } = RelateText.GenId();

    /// <summary>
    /// Mã đặt chỗ
    /// </summary>
    public string? BookingCode { get; set; }
    
    public string? AbTripFareDataId { get; set; }
    public string? Airline { get; set; }
    public string? Operating { get; set; }
    
    public int? TotalPrice { get; set; }
    
    public int? Adt { get; set; }
    public int? FareAdt { get; set; }
    public int? TaxAdt { get; set; }
    public int? FeeAdt { get; set; }
    public int? ServiceFeeAdt { get; set; }
    
    public int? Chd { get; set; }
    public int? FareChd { get; set; }
    public int? TaxChd { get; set; }
    public int? FeeChd { get; set; }
    public int? ServiceFeeChd { get; set; }
    
    public int? Inf { get; set; }
    public int? FareInf { get; set; }
    public int? TaxInf { get; set; }
    public int? FeeInf { get; set; }
    public int? ServiceFeeInf { get; set; }
    
    public string BillId { get; set; } = null!;
    public Model.Bill Bill { get; set; }
}