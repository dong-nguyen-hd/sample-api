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

    public async Task<BaseResult<List<AircraftsResponse>>> GetAircraftsAsync(CancellationToken cancellationToken = default)
    {
        var aircraftsDatta = await abTripService.GetAircraftsAsync(cancellationToken);

        // Xử lí với dữ liệu thành công từ AbTrip
        if (aircraftsDatta.CodeMessage == CodeMessage._99)
            return GetBaseResult(CodeMessage._99, data: Mapper.Map<List<AircraftsResponse>>(aircraftsDatta.Data));

        return GetBaseResult<List<AircraftsResponse>>(CodeMessage._100);
    }

    public async Task<BaseResult<List<AirlinesResponse>>> GetAirlinesAsync(CancellationToken cancellationToken = default)
    {
        var airlinesDatta = await abTripService.GetAirlinesAsync(cancellationToken);

        // Xử lí với dữ liệu thành công từ AbTrip
        if (airlinesDatta.CodeMessage == CodeMessage._99)
            return GetBaseResult(CodeMessage._99, data: Mapper.Map<List<AirlinesResponse>>(airlinesDatta.Data));

        return GetBaseResult<List<AirlinesResponse>>(CodeMessage._100);
    }

    public async Task<BaseResult<List<AirportsResponse>>> GetAirportsAsync(CancellationToken cancellationToken = default)
    {
        var airportsDatta = await abTripService.GetAirportsAsync(cancellationToken);

        // Xử lí với dữ liệu thành công từ AbTrip
        if (airportsDatta.CodeMessage == CodeMessage._99)
            return GetBaseResult(CodeMessage._99, data: Mapper.Map<List<AirportsResponse>>(airportsDatta.Data));

        return GetBaseResult<List<AirportsResponse>>(CodeMessage._100);
    }

    #region Search Flight

    public async Task<BaseResult<SearchResponse>> SearchAsync(SearchRequest request, CancellationToken cancellationToken = default)
    {
        var searchFlightData = await abTripService.SearchFlightAsync(Mapper.Map<AbTrip.Request.SearchFlightRequest>(request), cancellationToken);

        // Xử lí với dữ liệu thành công từ AbTrip
        if (searchFlightData.CodeMessage == CodeMessage._99 &&
            searchFlightData.Data!.Status!.Value &&
            searchFlightData.Data.ErrorCode == "000")
        {
            var getFareRulesData = await abTripService.GetFareRulesAsync(ComputeGetFareRulesRequest(searchFlightData.Data), cancellationToken);

            return GetBaseResult(CodeMessage._99, data: MappingSearchFlightResponse(searchFlightData.Data, getFareRulesData.Data));
        }

        return GetBaseResult<SearchResponse>(CodeMessage._100);
    }

    private SearchResponse MappingSearchFlightResponse(AbTrip.Response.SearchFlightResponse searchData, AbTrip.Response.GetFareRulesResponse? fareRulesData)
    {
        // Mapping search-flight
        var searchResponse = Mapper.Map<SearchResponse>(searchData);

        if (fareRulesData is null)
            return searchResponse;

        foreach (var fareData in searchResponse.ListFareData!)
        {
            // Mapping fare-rule
            var tempFareRule = fareRulesData.ListFareRules!.SingleOrDefault(x => x.FareDataInfo!.FareDataId == fareData.FareDataId);
            fareData.FareRules = Mapper.Map<FareRulesResponse>(tempFareRule);
        }

        return searchResponse;
    }

    private static AbTrip.Request.GetFareRulesRequest ComputeGetFareRulesRequest(AbTrip.Response.SearchFlightResponse resource)
    {
        List<AbTrip.Request.FareDataRequest> fareDataRequests = new();

        foreach (var fareData in resource.ListFareData!)
        {
            fareDataRequests.Add(new()
            {
                Session = resource.Session,
                FareDataId = fareData.FareDataId,
                ListFlight = fareData.ListFlight.Select(x => new AbTrip.Request.FlightRequest()
                {
                    FlightValue = x.FlightValue
                }).ToList()
            });
        }

        return new()
        {
            ListFareData = fareDataRequests
        };
    }

    #endregion

    #endregion
}