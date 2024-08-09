using EPAY.AIRWAY.KIOSK.API.Domain.Context;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;
using EPAY.AIRWAY.KIOSK.API.Resources.Exceptions;
using AbTrip = EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip;

namespace EPAY.AIRWAY.KIOSK.API.Services;

public sealed class FlightService(
    IConfigurationService configurationService,
    ICacheService cacheService,
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
        // Lấy dữ liệu từ cache nếu tồn tại
        string cacheKey = $"{nameof(FlightService)}-{DateTime.UtcNow:yyyy-MM-dd}-CHDaZuSZBD6a";

        var cacheData = await cacheService.GetDataAsync<MasterDataResponse>(cacheKey);
        if (cacheData != null)
            return GetBaseResult(CodeMessage._99, data: cacheData);
        ;

        // Lấy dữ liệu từ AbTrip khi cache không tồn tại
        await GetConfigDataAsync(cancellationToken);

        var aircraftsTask = abTripService.GetAircraftsAsync(cancellationToken);
        var airlinesTask = abTripService.GetAirlinesAsync(cancellationToken);
        var airportsTask = abTripService.GetAirportsAsync(cancellationToken);

        await Task.WhenAll(aircraftsTask, airlinesTask, airportsTask);

        if (aircraftsTask.Result.CodeMessage == CodeMessage._99 &&
            airlinesTask.Result.CodeMessage == CodeMessage._99 &&
            airportsTask.Result.CodeMessage == CodeMessage._99)
        {
            MasterDataResponse resultInner = new()
            {
                Aircrafts = MappingAircraftsResponse(aircraftsTask.Result.Data),
                Airlines = MappingAirlinesResponse(airlinesTask.Result.Data),
                Airports = MappingAirportsResponse(airportsTask.Result.Data),
            };
            await cacheService.SetDataAsync(cacheKey, resultInner, TimeSpan.FromDays(1));

            return GetBaseResult(CodeMessage._99, data: resultInner);
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

    public async Task<BaseResult<SearchResponseTemp>> SearchTempAsync(SearchRequest request, CancellationToken cancellationToken = default)
    {
        var searchFlightTask = abTripService.SearchFlightAsync(mapper.Map<AbTrip.Request.SearchFlightRequest>(request), cancellationToken);
        var masterDataTask = GetMasterDataAsync(cancellationToken);

        await Task.WhenAll(searchFlightTask, masterDataTask);

        // Xử lí với dữ liệu thành công từ AbTrip
        if (searchFlightTask.Result.CodeMessage == CodeMessage._99 &&
            searchFlightTask.Result.Data!.Status!.Value &&
            searchFlightTask.Result.Data.ErrorCode == "000" &&
            masterDataTask.Result.CodeMessage == CodeMessage._99)
        {
            var getFareRulesData = await abTripService.GetFareRulesAsync(ComputeGetFareRulesRequest(searchFlightTask.Result.Data), cancellationToken);

            return GetBaseResult(CodeMessage._99, data: MappingSearchFlightTempResponse(searchFlightTask.Result.Data, getFareRulesData.Data, masterDataTask.Result.Data!));
        }

        return GetBaseResult<SearchResponseTemp>(CodeMessage._100);
    }

    private SearchResponseTemp MappingSearchFlightTempResponse(AbTrip.Response.SearchFlightResponse searchData, AbTrip.Response.GetFareRulesResponse? fareRulesData, MasterDataResponse masterData)
    {
        // Mapping search-flight
        SearchResponseTemp result = new()
        {
            Session = searchData.Session,
            Itinerary = searchData.Itinerary,
            SearchDetail = new()
        };

        // Phân loại fligt-type
        result.FlightType = searchData switch
        {
            { FlightType: var x, Itinerary: var y } when x!.Equals("domestic", StringComparison.OrdinalIgnoreCase) && y == 1
                => MyEnum.FlightType.DomesticOneWay,
            { FlightType: var x, Itinerary: var y } when x!.Equals("domestic", StringComparison.OrdinalIgnoreCase) && y == 2
                => MyEnum.FlightType.DomesticTwoWay,
            { FlightType: var x, Itinerary: var y } when x!.Equals("international", StringComparison.OrdinalIgnoreCase) && y == 1
                => MyEnum.FlightType.InternationalOneWay,
            { FlightType: var x, Itinerary: var y } when x!.Equals("international", StringComparison.OrdinalIgnoreCase) && y == 2
                => MyEnum.FlightType.InternationalTwoWay,
            _ => MyEnum.FlightType.Other
        };

        // Gom nhóm dữ liệu
        // TH1: Với chuyến bay nội địa 1-2 chiều, quốc tế 1 chiều => gộp theo điều kiện flightValue và startDate
        // TH2: Với chuyến bay quốc tế 2 chiều => gộp theo fare-id

        if (result.FlightType == MyEnum.FlightType.InternationalTwoWay)
        {
        }
        else
        {
            // Gom nhóm dữ liệu
            List<GroupDataRequest> bucket = new();

            // Duyệt qua từng fare
            for (int i = 0; i < searchData.ListFareData!.Count; i++)
            {
                var fare = searchData.ListFareData[i];
                var flight = fare.ListFlight.First();

                // Duyệt qua từng group
                bool isContainParent = false;
                for (int j = 0; j < bucket.Count; j++)
                {
                    var current = bucket[j];

                    // Kiểm tra có tồn tại way không?
                    if (current.Way!.Equals($"{flight.StartPoint}-{flight.EndPoint}", StringComparison.OrdinalIgnoreCase))
                    {
                        isContainParent = true;

                        // Kiểm tra có tồn tại detect-flight không
                        bool isContainChild = false;
                        foreach (var tempDetectFlight in current.DetectFlight!)
                        {
                            // Tồn tại detect-flight thì thêm fare
                            if (tempDetectFlight.FlightNumber!.Equals(flight.FlightNumber!, StringComparison.OrdinalIgnoreCase) &&
                                tempDetectFlight.StartDate == flight.StartDate)
                            {
                                isContainChild = true;
                                tempDetectFlight.FareData!.Add(fare);
                                break;
                            }
                        }

                        // Chưa tồn tại detect-flight thì thêm mới
                        if (!isContainChild)
                        {
                            current.DetectFlight.Add(new()
                            {
                                FlightNumber = flight.FlightNumber,
                                StartDate = flight.StartDate,
                                FareData = [fare]
                            });

                            break;
                        }
                    }
                }

                // Thêm group nếu nó chưa tồn tại
                if (!isContainParent)
                {
                    bucket.Add(new()
                    {
                        Way = $"{flight.StartPoint}-{flight.EndPoint}",
                        DetectFlight =
                        [
                            new()
                            {
                                FlightNumber = flight.FlightNumber,
                                StartDate = flight.StartDate,
                                FareData = [fare]
                            }
                        ]
                    });
                }
            }

            // // Sử dụng dữ liệu đã gom nhóm
            // int index = 0;
            // AirportsResponse? startPoint = default;
            // AirportsResponse? endPoint = default;
            // List<FilghtDetailResponse>? listFlight = new();
            // foreach (var current in bucket)
            // {
            //     // Các biến chứa dữ liệu chung về thông tin chuyến bay
            //     // Ngoại trừ hạng vé, giá vé phải tính trong từng fare
            //     var fare = current.Item3[0];
            //     var flight = fare.ListFlight[0];
            //
            //     // Mapping flight
            //     FilghtDetailResponse tempFilghtDetail = new()
            //     {
            //         FlightStart = new()
            //         {
            //             Index = index,
            //             FlightNumber = flight.FlightNumber,
            //             FlightValue = flight.FlightValue,
            //             //airline
            //             //operating,
            //             StartDate = flight.StartDate,
            //             EndDate = flight.EndDate,
            //             Duration = flight.Duration,
            //             StopNum = flight.StopNum,
            //             HasUpgradeClass = current.Item3.Count > 1
            //         }
            //     };
            //
            //     // Mapping fare
            //     List<FareResponse>? listFare = new();
            //     foreach (var currentFare in current.Item3)
            //     {
            //         var tempFareRule = fareRulesData?.ListFareRules!.SingleOrDefault(x => x.FareDataInfo!.FareDataId == currentFare.FareDataId);
            //
            //         FareResponse tempFare = new()
            //         {
            //             FareDataId = currentFare.FareDataId,
            //             Adt = currentFare.Adt,
            //             Chd = currentFare.Chd,
            //             Inf = currentFare.Inf,
            //             UnitPriceAdt = 10000,
            //             UnitPriceChd = 11000,
            //             UnitPriceInf = 120000,
            //             TotalPrice = currentFare.TotalPrice,
            //             GroupClass = currentFare.ListFlight[0].GroupClass,
            //             FareClass = currentFare.ListFlight[0].FareClass,
            //             FareRules = mapper.Map<FareRulesResponse>(tempFareRule),
            //         };
            //
            //         // Mapping segment
            //         List<FlightSegmentResponse> listFlightSegment = new();
            //         foreach (var segment in currentFare.ListFlight[0].ListSegment)
            //         {
            //             FlightSegmentResponse tempSegment = new()
            //             {
            //                 FlightNumber = segment.FlightNumber,
            //                 //airline,
            //                 //operatin,
            //                 //startpoint,
            //                 //endpoint,
            //                 StartTime = segment.StartTime,
            //                 StartTimeZoneOffset = segment.StartTimeZoneOffset,
            //                 EndTime = segment.EndTime,
            //                 EndTimeZoneOffset = segment.EndTimeZoneOffset,
            //                 Duration = segment.Duration,
            //                 //plane,
            //                 Seat = segment.Seat,
            //                 Class = segment.Class,
            //                 HandBaggage = segment.HandBaggage,
            //                 AllowanceBaggage = segment.AllowanceBaggage
            //             };
            //             listFlightSegment.Add(tempSegment);
            //         }
            //
            //         tempFare.ListSegment = listFlightSegment;
            //         listFare.Add(tempFare);
            //     }
            //
            //     // Mapping airport
            //     foreach (var airport in masterData.Airports!)
            //     {
            //         if (flight.StartPoint!.Equals(airport.Code, StringComparison.OrdinalIgnoreCase))
            //             startPoint = airport;
            //         if (flight.EndPoint!.Equals(airport.Code, StringComparison.OrdinalIgnoreCase))
            //             endPoint = airport;
            //         if (startPoint != null && endPoint != null)
            //             break;
            //     }
            //
            //     // Mapping airline
            //     foreach (var airline in masterData.Airlines!)
            //     {
            //     }
            //
            //     listFlight.Add(tempFilghtDetail);
            //
            //     index++;
            // }
            //
            // result.SearchDetail.Add(new()
            // {
            //     StartPoint = startPoint,
            //     EndPoint = endPoint,
            //     ListFlight = listFlight
            // });
        }

        return result;
    }

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