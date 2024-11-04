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
    /// <param name="hasPopularity">Có/không lấy thông tin địa điểm phổ biến</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<MasterDataResponse>> GetMasterDataAsync(bool hasPopularity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: lấy thông tin về dịch vụ bổ sung, hành lí mua thêm
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<AdditionalServicesResponse>> GetAdditionalServicesAsync(AdditionalServicesRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: kiểm tra trạng thái fare trước khi booking
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<VerifyResponse>> VerifyAsync(VerifyRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: booking
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<BookingResponse>> BookingAsync(BookingRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: xuất vé
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<IssueResponse>> IssueAsync(IssueRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: tìm kiếm thông tin vé trả sau
    /// </summary>
    /// <param name="request"></param>
    /// <param name="utcNow"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<CheckOrderInfoResponse>> CheckOrderInfoAsync(CheckOrderInfoRequest request, DateTime utcNow, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: chuyển đổi loại chuyến bay -> loại vé
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    MyEnum.TicketType ConvertTicketType(MyEnum.FlightType source);

    /// <summary>
    /// Chức năng: chuyển đội loại chuyến bay -> loại hành trình
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    MyEnum.JourneyType ConvertJourneyType(MyEnum.FlightType source);
}