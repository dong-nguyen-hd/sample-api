using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip.Response;
using EPAY.AIRWAY.KIOSK.API.Resources.SystemData.ThirdParty.AbTrip;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Services;

public interface IAbTripService : IBaseService
{
    /// <summary>
    /// Chức năng: lấy thông tin về model máy bay
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<List<AircraftsResponse>>> GetAircraftsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: lấy thông tin về hãng hàng không
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<List<AirlinesResponse>>> GetAirlinesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: lấy thông tin về cảng hàng không
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<List<AirportsResponse>>> GetAirportsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: giữ chỗ -> đặt vé
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<BookFlightResponse>> BookFlightAsync(BookFlightRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: lấy thông tin chính sách vé
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<GetFareRulesResponse>> GetFareRulesAsync(GetFareRulesRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: kiểm tra giá trước khi thanh toán
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<PriceQuoteResponse>> PriceQuoteAsync(PriceQuoteRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: lấy thông tin chuyến bay
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<SearchFlightResponse>> SearchFlightAsync(SearchFlightRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: kiểm tra vé trước khi đặt chỗ
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<VerifyFlightResponse>> VerifyFlightAsync(VerifyFlightRequest request, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Chức năng: lấy thông tin cấu hình
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<AbTripInfo> GetConfigDataAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: lấy ra thông tin hành lí mua thêm
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<GetBaggageResponse>> GetBaggageAsync(GetBaggageRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: lấy ra thông tin dịch vụ khác mua thêm
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<GetAncillaryResponse>> GetAncillaryAsync(GetAncillaryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: xuất vé
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<IssueResponse>> IssueAsync(IssueRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: lấy thông tin vé luồng trả sau
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<OrderInfoResponse>> OrderInfoAsync(OrderInfoRequest request, CancellationToken cancellationToken = default);
}