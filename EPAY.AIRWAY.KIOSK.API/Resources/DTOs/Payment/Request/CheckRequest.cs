namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Payment.Request;

public sealed class CheckRequest
{
    public string OrderCode { get => _orderCode; set => _orderCode = value.ToLowerAndRemoveSpace(); }
    private string _orderCode;

    public string BillId { get => _billId; set => _billId = value.ToLowerAndRemoveSpace(); }
    private string _billId;
}