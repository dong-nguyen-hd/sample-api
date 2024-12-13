using BasePagination = EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Pagination.Request;

namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.IACV.Request;

public sealed class QueryDataRequest : BasePagination.QueryDataRequest<SearchRequest, OrderRequest>
{
    /// <summary>
    /// Nền tảng tích hợp
    /// </summary>
    public MyEnum.PlatformType? PlatformType { get; set; }
    
    /// <summary>
    /// Mã partner-key
    /// </summary>
    public string? PartnerKey { get; set; }
}