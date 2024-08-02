using EPAY.AIRWAY.KIOSK.API.Domain.Context;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;
using AbTrip = EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip;

namespace EPAY.AIRWAY.KIOSK.API.Services;

public sealed class FlightService(
    IAbTripService abTripService,
    IMapper mapper,
    CoreContext context) : BaseService(mapper, context), IFlightService
{
    #region Method

    public async Task<BaseResult<SearchResponse>> SearchAsync(SearchRequest request, CancellationToken cancellationToken = default)
    {
        var abTripSearchFlight = await abTripService.SearchFlightAsync(Mapper.Map<AbTrip.Request.SearchFlightRequest>(request), cancellationToken);

        if (abTripSearchFlight.CodeMessage == CodeMessage._99 &&
            abTripSearchFlight.Data.Status.Value &&
            abTripSearchFlight.Data.ErrorCode == "000")
            return GetBaseResult(CodeMessage._99, data: Mapper.Map<SearchResponse>(abTripSearchFlight.Data));

        return GetBaseResult<SearchResponse>(CodeMessage._100);
    }

    #endregion
}