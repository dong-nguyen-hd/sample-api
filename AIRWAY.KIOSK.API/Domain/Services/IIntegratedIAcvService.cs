using AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.IACV.Request;
using AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.IACV.Response;

namespace AIRWAY.KIOSK.API.Domain.Services;

public interface IIntegratedIAcvService : IBaseService
{
    /// <summary>
    /// Chức năng: lấy ra danh sách đơn hàng
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PaginationResult<QueryDataResponse>> GetByPartnerKeyAsync(QueryDataRequest request, CancellationToken cancellationToken = default);
}