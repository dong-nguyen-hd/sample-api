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
    /// Chức năng: lấy ra thông tin model máy bay
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<List<AircraftsResponse>>> GetAircraftsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: lấy ra thông tin hãng bay
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<List<AirlinesResponse>>> GetAirlinesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: lấy ra thông tin cảng hàng không
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<List<AirportsResponse>>> GetAirportsAsync(CancellationToken cancellationToken = default);
}