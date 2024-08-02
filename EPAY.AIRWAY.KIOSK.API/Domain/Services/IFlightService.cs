using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Services;

public interface IFlightService : IBaseService
{
    Task<BaseResult<SearchResponse>> SearchAsync(SearchRequest request, CancellationToken cancellationToken = default);
}