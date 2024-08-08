using EPAY.AIRWAY.KIOSK.API.Domain.Context;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;
using EPAY.AIRWAY.KIOSK.API.Resources.Exceptions;
using AbTrip = EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip;

namespace EPAY.AIRWAY.KIOSK.API.Services;

public sealed class FlightService(
    IConfigurationService configurationService,
    IAbTripService abTripService,
    IMapper mapper,
    CoreContext context) : BaseService, IFlightService
{
    #region Properties

    private string _hostBE = string.Empty;

    #endregion

    #region Method

    #region Master data

    public async Task<BaseResult<MasterDataResponse>> GetMasterDataAsync(CancellationToken cancellationToken = default)
    {
        await GetConfigDataAsync(cancellationToken);
        
        var aircraftsTask = abTripService.GetAircraftsAsync(cancellationToken);
        var airlinesTask = abTripService.GetAirlinesAsync(cancellationToken);
        var airportsTask = abTripService.GetAirportsAsync(cancellationToken);

        await Task.WhenAll(aircraftsTask, airlinesTask, airportsTask);

        if (aircraftsTask.Result.CodeMessage == CodeMessage._99 &&
            airlinesTask.Result.CodeMessage == CodeMessage._99 &&
            airportsTask.Result.CodeMessage == CodeMessage._99)
        {
            MasterDataResponse result = new()
            {
                Aircrafts = MappingAircraftsResponse(aircraftsTask.Result.Data),
                Airlines = MappingAirlinesResponse(airlinesTask.Result.Data),
                Airports = MappingAirportsResponse(airportsTask.Result.Data),
            };

            return GetBaseResult(CodeMessage._99, data: result);
        }

        return GetBaseResult<MasterDataResponse>(CodeMessage._100);
    }

    private static List<AircraftsResponse>? MappingAircraftsResponse(List<AbTrip.Response.AircraftsResponse>? resource)
    {
        if (resource is null || resource.Count == 0)
            return default;

        List<AircraftsResponse> result = new(resource.Count);
        foreach (var item in resource)
        {
            result.Add(new()
            {
                Code = item.Code,
                Manufacturer = item.Manufacturer,
                Model = item.Model
            });
        }

        return result;
    }

    private List<AirlinesResponse>? MappingAirlinesResponse(List<AbTrip.Response.AirlinesResponse>? resource, MyEnum.LanguageType languageType = MyEnum.LanguageType.Vietnam)
    {
        if (resource is null || resource.Count == 0)
            return default;

        List<AirlinesResponse> result = new(resource.Count);
        foreach (var item in resource)
        {
            result.Add(new()
            {
                Code = item.Code,
                Name = languageType == MyEnum.LanguageType.Vietnam ? item.Name : item.NameEn,
                Logo = $"{_hostBE}/resources/airline/{item.Logo}"
            });
        }

        return result;
    }

    private static List<AirportsResponse>? MappingAirportsResponse(List<AbTrip.Response.AirportsResponse>? resource, MyEnum.LanguageType languageType = MyEnum.LanguageType.Vietnam)
    {
        if (resource is null || resource.Count == 0)
            return default;

        List<AirportsResponse> result = new(resource.Count);
        foreach (var item in resource)
        {
            result.Add(new()
            {
                Code = item.Code,
                Name = languageType == MyEnum.LanguageType.Vietnam ? item.NameVi : item.NameEn,
                CityName = languageType == MyEnum.LanguageType.Vietnam ? item.CityNameVi : item.CityNameEn,
                CountryName = languageType == MyEnum.LanguageType.Vietnam ? item.CountryNameVi : item.CountryNameEn,
                CountryCode = item.CountryCode,
            });
        }

        return result;
    }

    #endregion

    #region Search Flight

    public async Task<BaseResult<SearchResponse>> SearchAsync(SearchRequest request, CancellationToken cancellationToken = default)
    {
        var searchFlightData = await abTripService.SearchFlightAsync(mapper.Map<AbTrip.Request.SearchFlightRequest>(request), cancellationToken);

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
        var searchResponse = mapper.Map<SearchResponse>(searchData);

        if (fareRulesData is null)
            return searchResponse;

        foreach (var fareData in searchResponse.ListFareData!)
        {
            // Mapping fare-rule
            var tempFareRule = fareRulesData.ListFareRules!.SingleOrDefault(x => x.FareDataInfo!.FareDataId == fareData.FareDataId);
            fareData.FareRules = mapper.Map<FareRulesResponse>(tempFareRule);
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

    #region Private work

    private async Task GetConfigDataAsync(CancellationToken cancellationToken = default)
    {
        // Get config from DB
        var configurations = await configurationService.GetAllAsync(false, cancellationToken);

        if (configurations.CodeMessage != CodeMessage._99)
            throw new MessageResultException("Không thể thực hiện lấy config");

        foreach (var configuration in configurations.Data!)
        {
            // Config
            if (configuration.Key == SystemConfig.SystemBeHost)
            {
                this._hostBE = configuration.Value!;
                break;
            }
        }
    }

    #endregion

    #endregion
}