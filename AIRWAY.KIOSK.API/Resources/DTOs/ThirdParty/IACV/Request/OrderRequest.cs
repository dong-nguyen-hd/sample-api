using AIRWAY.KIOSK.API.Resources.DTOs.Pagination.Request;

namespace AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.IACV.Request;

public sealed class OrderRequest : SortRequest
{
    public MyEnum.OrderType? OrderType { get; set; }
}