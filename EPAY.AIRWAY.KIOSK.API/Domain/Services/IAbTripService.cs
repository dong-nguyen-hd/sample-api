using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip.Response;
using EPAY.AIRWAY.KIOSK.API.Resources.SystemData.ThirdParty.AbTrip;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Services;

public interface IAbTripService : IBaseService
{
    Task<BaseResult<List<AircraftsResponse>>> GetAircraftsAsync(CancellationToken cancellationToken = default);

    Task<BaseResult<List<AirlinesResponse>>> GetAirlinesAsync(CancellationToken cancellationToken = default);

    Task<BaseResult<List<AirportsResponse>>> GetAirportsAsync(CancellationToken cancellationToken = default);

    Task<BaseResult<BookFlightResponse>> BookFlightAsync(BookFlightRequest request, CancellationToken cancellationToken = default);

    Task<BaseResult<GetFareRulesResponse>> GetFareRulesAsync(GetFareRulesRequest request, CancellationToken cancellationToken = default);

    Task<BaseResult<PriceQuoteResponse>> PriceQuoteAsync(PriceQuoteRequest request, CancellationToken cancellationToken = default);

    Task<BaseResult<SearchFlightResponse>> SearchFlightAsync(SearchFlightRequest request, CancellationToken cancellationToken = default);

    Task<BaseResult<VerifyFlightResponse>> VerifyFlightAsync(VerifyFlightRequest request, CancellationToken cancellationToken = default);
    
    Task<AbTripInfo> GetConfigDataAsync(CancellationToken cancellationToken = default);
}