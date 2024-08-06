using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Services;

public interface IFlightService : IBaseService
{
    /// <summary>
    /// Chức năng: tìm kiếm danh sách chuyến bay
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<SearchResponse>> SearchAsync(SearchRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: lấy thông tin về model, hãng bay, cảng hàng không
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<MasterDataResponse>> GetMasterDataAsync(CancellationToken cancellationToken = default);
}