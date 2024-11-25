using EPAY.AIRWAY.KIOSK.API.Domain.Context;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;
using EPAY.AIRWAY.KIOSK.API.Resources.Exceptions;
using Microsoft.EntityFrameworkCore;
using AbTrip = EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip;

namespace EPAY.AIRWAY.KIOSK.API.Services;

public sealed class FlightService(
    IConfigurationService configurationService,
    ICacheService cacheService,
    IAbTripService abTripService,
    IHttpContextAccessor httpContextAccessor,
    IMapper mapper,
    CoreContext context) : BaseService, IFlightService
{
    #region Properties

    private readonly HttpContext? _httpContext = httpContextAccessor?.HttpContext;
    private string _hostBE = string.Empty;

    #endregion

    #region Method

    #region Master data

    public async Task<BaseResult<MasterDataResponse>> GetMasterDataAsync(bool hasPopularity, CancellationToken cancellationToken = default)
    {
        // Lấy dữ liệu từ cache nếu tồn tại
        string cacheKey = $"{nameof(FlightService)}-{DateTime.UtcNow:yyyy-MM-dd}-CHDaZuSZBD6a";

        var cacheData = await cacheService.GetDataAsync<MasterDataResponse>(cacheKey);
        if (cacheData != null)
        {
            if (hasPopularity)
                cacheData.Popularity = await ComputePopularity(cacheData.Airports!);
            return GetBaseResult(CodeMessage._0000, data: cacheData);
        }

        // Lấy dữ liệu từ AbTrip khi cache không tồn tại
        await GetConfigDataAsync(cancellationToken);

        var aircraftsTask = abTripService.GetAircraftsAsync(cancellationToken);
        var airlinesTask = abTripService.GetAirlinesAsync(cancellationToken);
        var airportsTask = abTripService.GetAirportsAsync(cancellationToken);

        await Task.WhenAll(aircraftsTask, airlinesTask, airportsTask);

        if (aircraftsTask.Result.CodeMessage == CodeMessage._0000 &&
            airlinesTask.Result.CodeMessage == CodeMessage._0000 &&
            airportsTask.Result.CodeMessage == CodeMessage._0000)
        {
            MasterDataResponse resultInner = new()
            {
                Aircrafts = MappingAircraftsResponse(aircraftsTask.Result.Data),
                Airlines = MappingAirlinesResponse(airlinesTask.Result.Data),
                Airports = MappingAirportsResponse(airportsTask.Result.Data),
            };
            await cacheService.SetDataAsync(cacheKey, resultInner, TimeSpan.FromDays(1));

            if (hasPopularity)
                resultInner.Popularity = await ComputePopularity(resultInner.Airports!);
            return GetBaseResult(CodeMessage._0000, data: resultInner);
        }

        if (aircraftsTask.Result.CodeMessage != CodeMessage._0000)
            return GetBaseResult<MasterDataResponse>(aircraftsTask.Result.CodeMessage);
        if (airlinesTask.Result.CodeMessage != CodeMessage._0000)
            return GetBaseResult<MasterDataResponse>(airlinesTask.Result.CodeMessage);

        return GetBaseResult<MasterDataResponse>(airportsTask.Result.CodeMessage);
    }

    /// <summary>
    /// Chức năng: tạo dữ liệu cảng hàng không phổ biến
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    private async Task<List<AirportResponse>> ComputePopularity(List<AirportResponse> source)
    {
        List<AirportResponse> result = new();
        HashSet<string> keys = new();

        // Tìm các địa điểm start-point phổ biến trong Reservation
        var popularity = await context.FlightDatas.GroupBy(x => x.StartPoint).Select(x => new
        {
            StartPoint = x.Key,
            Count = x.Count()
        }).OrderByDescending(x => x.Count).Take(4).ToListAsync();

        foreach (var item in popularity)
        {
            keys.Add(item.StartPoint ?? string.Empty);
        }

        keys.Add("HAN");
        keys.Add("SGN");
        keys.Add("DAD");
        keys.Add("CXR");
        var cleanKeys = keys.Take(4).ToArray();

        foreach (var airport in source)
        foreach (var key in cleanKeys)
            if (airport.Code!.Equals(key, StringComparison.OrdinalIgnoreCase))
                result.Add(airport);

        return result;
    }

    private static List<AircraftResponse>? MappingAircraftsResponse(List<AbTrip.Response.AircraftsResponse>? resource)
    {
        if (resource is null || resource.Count == 0)
            return default;

        List<AircraftResponse> result = new(resource.Count);
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

    private List<AirlineResponse>? MappingAirlinesResponse(List<AbTrip.Response.AirlinesResponse>? resource, MyEnum.LanguageType languageType = MyEnum.LanguageType.Vietnam)
    {
        if (resource is null || resource.Count == 0)
            return default;

        List<AirlineResponse> result = new(resource.Count);
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

    private static List<AirportResponse>? MappingAirportsResponse(List<AbTrip.Response.AirportsResponse>? resource, MyEnum.LanguageType languageType = MyEnum.LanguageType.Vietnam)
    {
        if (resource is null || resource.Count == 0)
            return default;

        List<AirportResponse> result = new(resource.Count);
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
        var masterDataTask = GetMasterDataAsync(false, cancellationToken);
        var searchFlightTask = abTripService.SearchFlightAsync(mapper.Map<AbTrip.Request.SearchFlightRequest>(request), cancellationToken);

        await Task.WhenAll(masterDataTask, searchFlightTask);

        // Xử lí với dữ liệu thành công từ AbTrip
        if (searchFlightTask.Result.CodeMessage == CodeMessage._0000 &&
            masterDataTask.Result.CodeMessage == CodeMessage._0000)
        {
            var searchFlightData = CleanRawFlightAbTrip(searchFlightTask.Result.Data!);

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var fareRulesAbTrip = await abTripService.GetFareRulesAsync(ComputeGetFareRulesRequest(searchFlightData), cts.Token);
            var cleanFareRulesAbTrip = CleanFareRulesAbTrip(fareRulesAbTrip);
            var flightResult = MappingSearchFlightResponse(searchFlightData, cleanFareRulesAbTrip, masterDataTask.Result.Data!);

            // Xử lí mã lỗi cho chuến bay nội địa khứ hồi thiếu thông tin chiều đi hoặc chiều về
            if (flightResult is { FlightType: MyEnum.FlightType.DomesticRoundTrip, SearchDetail.Count: <= 1 })
                return GetBaseResult<SearchResponse>(CodeMessage._5001);

            return GetBaseResult(CodeMessage._0000, data: flightResult);
        }

        if (searchFlightTask.Result.CodeMessage != CodeMessage._0000)
            return GetBaseResult<SearchResponse>(searchFlightTask.Result.CodeMessage);

        return GetBaseResult<SearchResponse>(masterDataTask.Result.CodeMessage);
    }

    /// <summary>
    /// Chức năng: làm sạch dữ liệu chuyến bay từ AbTrip
    /// </summary>
    /// <param name="searchData"></param>
    /// <returns></returns>
    private static AbTrip.Response.SearchFlightResponse CleanRawFlightAbTrip(AbTrip.Response.SearchFlightResponse searchData)
    {
        MyEnum.FlightType flightType = MappingFlightType(searchData);

        // Khởi tạo dữ liệu trả về
        AbTrip.Response.SearchFlightResponse result = new()
        {
            FlightType = searchData.FlightType,
            Session = searchData.Session,
            Itinerary = searchData.Itinerary,
            ListFareData = new()
        };

        // Lọc ra tất cả các phần tử không hợp lệ
        var count = searchData.ListFareData!.Count;
        for (int i = 0; i < count; i++)
        {
            var fare = searchData.ListFareData[i];

            if (fare.ListFlight == null || fare.ListFlight.Count <= 0)
                continue;

            if (flightType == MyEnum.FlightType.InternationalRoundTrip && fare.ListFlight.Count != 2)
                continue;

            result.ListFareData.Add(fare);
        }

        return result;
    }

    /// <summary>
    /// Chức năng: làm sạch dữ liệu fare-rules từ Abtrip
    /// </summary>
    /// <param name="resource"></param>
    /// <returns></returns>
    private static AbTrip.Response.GetFareRulesResponse? CleanFareRulesAbTrip(BaseResult<AbTrip.Response.GetFareRulesResponse> resource)
    {
        if (resource.CodeMessage != CodeMessage._0000 ||
            resource?.Data == null ||
            resource?.Data?.ListFareRules == null ||
            resource.Data?.ListFareRules?.Count <= 0)
            return default;

        // Remove duplicate fare-rules
        HashSet<string> key = new();
        AbTrip.Response.GetFareRulesResponse fareRules = new()
        {
            ListFareRules = new()
        };

        int count = resource.Data!.ListFareRules!.Count;
        for (int i = 0; i < count; i++)
        {
            var fareRule = resource.Data!.ListFareRules[i];
            var tempKey = fareRule!.FareDataInfo!.FareDataId.ToString();
            if (!string.IsNullOrEmpty(tempKey) && key.Add(tempKey))
                fareRules.ListFareRules.Add(fareRule);
        }

        return fareRules;
    }

    /// <summary>
    /// Chức năng: phân loại flight-type
    /// </summary>
    /// <param name="searchData"></param>
    /// <returns></returns>
    private static MyEnum.FlightType MappingFlightType(AbTrip.Response.SearchFlightResponse searchData)
    {
        return searchData switch
        {
            { FlightType: var x, Itinerary: var y } when x!.Equals("domestic", StringComparison.OrdinalIgnoreCase) && y == 1
                => MyEnum.FlightType.DomesticOneWay,
            { FlightType: var x, Itinerary: var y } when x!.Equals("domestic", StringComparison.OrdinalIgnoreCase) && y == 2
                => MyEnum.FlightType.DomesticRoundTrip,
            { FlightType: var x, Itinerary: var y } when x!.Equals("international", StringComparison.OrdinalIgnoreCase) && y == 1
                => MyEnum.FlightType.InternationalOneWay,
            { FlightType: var x, Itinerary: var y } when x!.Equals("international", StringComparison.OrdinalIgnoreCase) && y == 2
                => MyEnum.FlightType.InternationalRoundTrip,
            _ => MyEnum.FlightType.Other
        };
    }

    /// <summary>
    /// Chức năng: tạo cấu trúc dữ liệu trả về thông tin chuyến bay
    /// </summary>
    /// <param name="searchData"></param>
    /// <param name="fareRulesData"></param>
    /// <param name="masterData"></param>
    /// <returns></returns>
    private SearchResponse MappingSearchFlightResponse(AbTrip.Response.SearchFlightResponse searchData, AbTrip.Response.GetFareRulesResponse? fareRulesData, MasterDataResponse masterData)
    {
        // Mapping search-flight
        SearchResponse result = new()
        {
            Session = searchData.Session,
            FlightType = MappingFlightType(searchData)
        };

        // Gom nhóm dữ liệu
        // TH1: Với chuyến bay nội địa 1-2 chiều, quốc tế 1 chiều => gộp theo điều kiện flight-number và start-date
        // TH2: Với chuyến bay quốc tế 2 chiều => gộp theo fare-data-id
        if (result.FlightType == MyEnum.FlightType.InternationalRoundTrip)
            result.SearchDetail = MappingInternationalTwoWayData(searchData, fareRulesData, masterData);
        else
            result.SearchDetail = MappingDomesticAndOtherData(searchData, fareRulesData, masterData);

        return result;
    }

    /// <summary>
    /// Chức năng: gán dữ liệu cho các chuyến bay quốc tế khứ hồi <br/>
    /// Với chuyến bay quốc tế 2 chiều => gộp theo fare-data-id
    /// </summary>
    /// <param name="searchData"></param>
    /// <param name="fareRulesData"></param>
    /// <param name="masterData"></param>
    /// <returns></returns>
    private List<SearchDetailResponse> MappingInternationalTwoWayData(AbTrip.Response.SearchFlightResponse searchData, AbTrip.Response.GetFareRulesResponse? fareRulesData, MasterDataResponse masterData)
    {
        Dictionary<string, AircraftResponse?> aircrafts = new();
        Dictionary<string, AirlineResponse?> airlines = new();
        Dictionary<string, AirportResponse?> airports = new();

        List<SearchDetailResponse> result = new();

        int index = 0;

        // Duyệt qua từng fare (abtrip)
        foreach (var fare in searchData.ListFareData!)
        {
            // Xác định đâu là chuyến bay chiều đi/chiều về trong danh sách chuyến bay
            AbTrip.Response.FlightResponse flightOne; // Tương ứng với chuyến bay chiều đi
            AbTrip.Response.FlightResponse flightTwo; // Tương ứng với chuyến bay chiều về
            if (fare.ListFlight[0].Leg == 0)
            {
                flightOne = fare.ListFlight[0];
                flightTwo = fare.ListFlight[1];
            }
            else
            {
                flightOne = fare.ListFlight[1];
                flightTwo = fare.ListFlight[0];
            }

            SearchDetailResponse searchDetail = new();

            // searchDetail:startPoint
            airports.TryAdd(flightOne.StartPoint!, default);
            searchDetail.StartPoint = new()
            {
                Code = flightOne.StartPoint
            };

            // searchDetail:endPoint
            airports.TryAdd(flightOne.EndPoint!, default);
            searchDetail.EndPoint = new()
            {
                Code = flightOne.EndPoint
            };

            // searchDetail:listFlight
            List<FilghtDetailResponse>? listFlightDetail = new();

            // Mapping flight
            airlines.TryAdd(flightOne.Airline!, default);
            airlines.TryAdd(flightOne.Operating!, default);
            airlines.TryAdd(flightTwo.Airline!, default);
            airlines.TryAdd(flightTwo.Operating!, default);
            FilghtDetailResponse filghtDetail = new()
            {
                FlightStart = new()
                {
                    Index = index,
                    FlightNumber = flightOne.FlightNumber,
                    Airline = new()
                    {
                        Code = flightOne.Airline
                    },
                    Operating = new()
                    {
                        Code = flightOne.Operating
                    },
                    StartDate = flightOne.StartDate,
                    EndDate = flightOne.EndDate,
                    Duration = flightOne.Duration,
                    StopNum = flightOne.StopNum,
                    HasUpgradeClass = false
                },
                FlightEnd = new()
                {
                    Index = index,
                    FlightNumber = flightTwo.FlightNumber,
                    Airline = new()
                    {
                        Code = flightTwo.Airline
                    },
                    Operating = new()
                    {
                        Code = flightTwo.Operating
                    },
                    StartDate = flightTwo.StartDate,
                    EndDate = flightTwo.EndDate,
                    Duration = flightTwo.Duration,
                    StopNum = flightTwo.StopNum,
                    HasUpgradeClass = false
                }
            };

            // Mapping flightStart:listFareClass
            List<FareResponse>? listFareOne = new();
            List<FareResponse>? listFareTwo = new();
            var tempFareRule = fareRulesData?.ListFareRules?.SingleOrDefault(x => x.FareDataInfo?.FareDataId == fare.FareDataId);

            FareResponse tempFareOne = new()
            {
                FlightValue = flightOne.FlightValue,
                FareDataId = fare.FareDataId,
                Adt = fare.Adt,
                Chd = fare.Chd,
                Inf = fare.Inf,
                UnitPriceAdt = fare.FareAdt + fare.TaxAdt + fare.FeeAdt + fare.ServiceFeeAdt,
                UnitPriceChd = fare.FareChd + fare.TaxChd + fare.FeeChd + fare.ServiceFeeChd,
                UnitPriceInf = fare.FareInf + fare.TaxInf + fare.FeeInf + fare.ServiceFeeInf,
                TotalPrice = fare.TotalPrice,
                GroupClass = flightOne.GroupClass,
                FareClass = flightOne.FareClass,
                FareRules = mapper.Map<FareRulesResponse>(tempFareRule),
            };
            FareResponse tempFareTwo = new()
            {
                FlightValue = flightTwo.FlightValue,
                FareDataId = fare.FareDataId,
                Adt = fare.Adt,
                Chd = fare.Chd,
                Inf = fare.Inf,
                UnitPriceAdt = fare.FareAdt + fare.TaxAdt + fare.FeeAdt + fare.ServiceFeeAdt,
                UnitPriceChd = fare.FareChd + fare.TaxChd + fare.FeeChd + fare.ServiceFeeChd,
                UnitPriceInf = fare.FareInf + fare.TaxInf + fare.FeeInf + fare.ServiceFeeInf,
                TotalPrice = fare.TotalPrice,
                GroupClass = flightTwo.GroupClass,
                FareClass = flightTwo.FareClass,
                FareRules = mapper.Map<FareRulesResponse>(tempFareRule),
            };

            // Mapping segment
            List<FlightSegmentResponse> listFlightSegmentOne = new();
            foreach (var segmentOne in flightOne.ListSegment)
            {
                airlines.TryAdd(segmentOne.Airline!, default);
                airlines.TryAdd(segmentOne.OperatingAirline!, default);
                airports.TryAdd(segmentOne.StopPoint ?? string.Empty, default);
                airports.TryAdd(segmentOne.StartPoint!, default);
                airports.TryAdd(segmentOne.EndPoint!, default);
                aircrafts.TryAdd(segmentOne.Plane!, default);

                FlightSegmentResponse tempSegment = new()
                {
                    Airline = new()
                    {
                        Code = segmentOne.Airline
                    },
                    Operating = new()
                    {
                        Code = segmentOne.OperatingAirline
                    },
                    StartPoint = new()
                    {
                        Code = segmentOne.StartPoint
                    },
                    EndPoint = new()
                    {
                        Code = segmentOne.EndPoint
                    },
                    StopPoint = new()
                    {
                        Code = segmentOne.StopPoint
                    },
                    StopTime = segmentOne.StopTime,
                    FlightNumber = segmentOne.FlightNumber,
                    StartTime = segmentOne.StartTime,
                    StartTimeZoneOffset = segmentOne.StartTimeZoneOffset,
                    EndTime = segmentOne.EndTime,
                    EndTimeZoneOffset = segmentOne.EndTimeZoneOffset,
                    Duration = segmentOne.Duration,
                    Plane = new()
                    {
                        Code = segmentOne.Plane
                    },
                    Seat = segmentOne.Seat,
                    Class = segmentOne.Class,
                    HandBaggage = segmentOne.HandBaggage,
                    AllowanceBaggage = segmentOne.AllowanceBaggage
                };
                listFlightSegmentOne.Add(tempSegment);
            }

            List<FlightSegmentResponse> listFlightSegmentTwo = new();
            foreach (var segmentTwo in flightTwo.ListSegment)
            {
                airlines.TryAdd(segmentTwo.Airline!, default);
                airlines.TryAdd(segmentTwo.OperatingAirline!, default);
                airports.TryAdd(segmentTwo.StopPoint ?? string.Empty, default);
                airports.TryAdd(segmentTwo.StartPoint!, default);
                airports.TryAdd(segmentTwo.EndPoint!, default);
                aircrafts.TryAdd(segmentTwo.Plane!, default);

                FlightSegmentResponse tempSegment = new()
                {
                    Airline = new()
                    {
                        Code = segmentTwo.Airline
                    },
                    Operating = new()
                    {
                        Code = segmentTwo.OperatingAirline
                    },
                    StartPoint = new()
                    {
                        Code = segmentTwo.StartPoint
                    },
                    EndPoint = new()
                    {
                        Code = segmentTwo.EndPoint
                    },
                    StopPoint = new()
                    {
                        Code = segmentTwo.StopPoint
                    },
                    StopTime = segmentTwo.StopTime,
                    FlightNumber = segmentTwo.FlightNumber,
                    StartTime = segmentTwo.StartTime,
                    StartTimeZoneOffset = segmentTwo.StartTimeZoneOffset,
                    EndTime = segmentTwo.EndTime,
                    EndTimeZoneOffset = segmentTwo.EndTimeZoneOffset,
                    Duration = segmentTwo.Duration,
                    Plane = new()
                    {
                        Code = segmentTwo.Plane
                    },
                    Seat = segmentTwo.Seat,
                    Class = segmentTwo.Class,
                    HandBaggage = segmentTwo.HandBaggage,
                    AllowanceBaggage = segmentTwo.AllowanceBaggage
                };
                listFlightSegmentTwo.Add(tempSegment);
            }

            // Tổng hợp dữ liệu
            tempFareOne.ListSegment = listFlightSegmentOne;
            tempFareTwo.ListSegment = listFlightSegmentTwo;
            listFareOne.Add(tempFareOne);
            listFareTwo.Add(tempFareTwo);

            filghtDetail.FlightStart.ListFareClass = listFareOne;
            filghtDetail.FlightEnd.ListFareClass = listFareTwo;
            listFlightDetail.Add(filghtDetail);

            searchDetail.ListFlight = listFlightDetail;
            result.Add(searchDetail);

            index++;
        }

        return MappingMasterDataInFlight(result, masterData, aircrafts, airlines, airports);
    }

    /// <summary>
    /// Chức năng: gán dữ liệu cho các chuyến bay nội địa 1-2, quốc tế 1 <br/>
    /// Với chuyến bay nội địa 1-2 chiều, quốc tế 1 chiều => gộp theo điều kiện flight-number và start-date
    /// </summary>
    /// <param name="searchData"></param>
    /// <param name="fareRulesData"></param>
    /// <param name="masterData"></param>
    /// <returns></returns>
    private List<SearchDetailResponse> MappingDomesticAndOtherData(AbTrip.Response.SearchFlightResponse searchData, AbTrip.Response.GetFareRulesResponse? fareRulesData, MasterDataResponse masterData)
    {
        // Gom nhóm dữ liệu
        List<GroupDataRequest> bucket = new();

        // Duyệt qua từng fare (abtrip)
        foreach (var fare in searchData.ListFareData!)
        {
            var flight = fare.ListFlight.First();

            // Duyệt qua từng group
            bool isContainParent = false;
            foreach (var current in bucket)
            {
                // Kiểm tra có tồn tại chiều đi/về hay không?
                if (current.StartPoint!.Equals(flight.StartPoint, StringComparison.OrdinalIgnoreCase) &&
                    current.EndPoint!.Equals(flight.EndPoint, StringComparison.OrdinalIgnoreCase))
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
                    StartPoint = flight.StartPoint,
                    EndPoint = flight.EndPoint,
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

        // Sử dụng dữ liệu đã gom nhóm
        Dictionary<string, AircraftResponse?> aircrafts = new();
        Dictionary<string, AirlineResponse?> airlines = new();
        Dictionary<string, AirportResponse?> airports = new();

        List<SearchDetailResponse> result = new();

        int index = 0;

        foreach (var group in bucket)
        {
            SearchDetailResponse searchDetail = new();

            // searchDetail:startPoint
            airports.TryAdd(group.StartPoint!, default);
            searchDetail.StartPoint = new()
            {
                Code = group.StartPoint
            };

            // searchDetail:endPoint
            airports.TryAdd(group.EndPoint!, default);
            searchDetail.EndPoint = new()
            {
                Code = group.EndPoint
            };

            // searchDetail:listFlight
            List<FilghtDetailResponse>? listFlightDetail = new();

            foreach (var innerGroup in group.DetectFlight!)
            {
                // tempFlight sử dụng để lấy các thông tin cơ bản của chuyến bay
                // Cần xử lí riêng với giá trị flightValue vì nó có sự thay đổi với mỗi fare
                var tempFlight = innerGroup.FareData![0].ListFlight[0];

                // Mapping flight
                airlines.TryAdd(tempFlight.Airline!, default);
                airlines.TryAdd(tempFlight.Operating!, default);
                FilghtDetailResponse filghtDetail = new()
                {
                    FlightStart = new()
                    {
                        Index = index,
                        FlightNumber = innerGroup.FlightNumber,
                        Airline = new()
                        {
                            Code = tempFlight.Airline
                        },
                        Operating = new()
                        {
                            Code = tempFlight.Operating
                        },
                        StartDate = tempFlight.StartDate,
                        EndDate = tempFlight.EndDate,
                        Duration = tempFlight.Duration,
                        StopNum = tempFlight.StopNum,
                        HasUpgradeClass = innerGroup.FareData.Count > 1
                    }
                };

                // Mapping flightStart:listFareClass
                List<FareResponse>? listFare = new();
                foreach (var currentFare in innerGroup.FareData)
                {
                    var tempFareRule = fareRulesData?.ListFareRules?.SingleOrDefault(x => x.FareDataInfo?.FareDataId == currentFare.FareDataId);

                    // Lấy thông tin flight tương ứng với từng fare
                    var flight = currentFare.ListFlight[0];

                    FareResponse tempFare = new()
                    {
                        FlightValue = flight.FlightValue,
                        FareDataId = currentFare.FareDataId,
                        Adt = currentFare.Adt,
                        Chd = currentFare.Chd,
                        Inf = currentFare.Inf,
                        UnitPriceAdt = currentFare.FareAdt + currentFare.TaxAdt + currentFare.FeeAdt + currentFare.ServiceFeeAdt,
                        UnitPriceChd = currentFare.FareChd + currentFare.TaxChd + currentFare.FeeChd + currentFare.ServiceFeeChd,
                        UnitPriceInf = currentFare.FareInf + currentFare.TaxInf + currentFare.FeeInf + currentFare.ServiceFeeInf,
                        TotalPrice = currentFare.TotalPrice,
                        GroupClass = currentFare.ListFlight[0].GroupClass,
                        FareClass = currentFare.ListFlight[0].FareClass,
                        FareRules = mapper.Map<FareRulesResponse>(tempFareRule),
                    };

                    // Mapping segment
                    List<FlightSegmentResponse> listFlightSegment = new();
                    foreach (var segment in currentFare.ListFlight[0].ListSegment)
                    {
                        airlines.TryAdd(segment.Airline!, default);
                        airlines.TryAdd(segment.OperatingAirline!, default);
                        airports.TryAdd(segment.StopPoint ?? string.Empty, default);
                        airports.TryAdd(segment.StartPoint!, default);
                        airports.TryAdd(segment.EndPoint!, default);
                        aircrafts.TryAdd(segment.Plane!, default);

                        FlightSegmentResponse tempSegment = new()
                        {
                            Airline = new()
                            {
                                Code = segment.Airline
                            },
                            Operating = new()
                            {
                                Code = segment.OperatingAirline
                            },
                            StartPoint = new()
                            {
                                Code = segment.StartPoint
                            },
                            EndPoint = new()
                            {
                                Code = segment.EndPoint
                            },
                            StopPoint = new()
                            {
                                Code = segment.StopPoint
                            },
                            StopTime = segment.StopTime,
                            FlightNumber = segment.FlightNumber,
                            StartTime = segment.StartTime,
                            StartTimeZoneOffset = segment.StartTimeZoneOffset,
                            EndTime = segment.EndTime,
                            EndTimeZoneOffset = segment.EndTimeZoneOffset,
                            Duration = segment.Duration,
                            Plane = new()
                            {
                                Code = segment.Plane
                            },
                            Seat = segment.Seat,
                            Class = segment.Class,
                            HandBaggage = segment.HandBaggage,
                            AllowanceBaggage = segment.AllowanceBaggage
                        };
                        listFlightSegment.Add(tempSegment);
                    }

                    tempFare.ListSegment = listFlightSegment;
                    listFare.Add(tempFare);
                }

                filghtDetail.FlightStart.ListFareClass = listFare;
                listFlightDetail.Add(filghtDetail);

                index++;
            }

            searchDetail.ListFlight = listFlightDetail;
            result.Add(searchDetail);
        }

        return MappingMasterDataInFlight(result, masterData, aircrafts, airlines, airports);
    }

    /// <summary>
    /// Chức năng: gán dữ liệu thông tin master-data có trong từng flight
    /// </summary>
    /// <param name="result"></param>
    /// <param name="masterData"></param>
    /// <param name="aircrafts"></param>
    /// <param name="airlines"></param>
    /// <param name="airports"></param>
    /// <returns></returns>
    private static List<SearchDetailResponse> MappingMasterDataInFlight(List<SearchDetailResponse> result, MasterDataResponse masterData, Dictionary<string, AircraftResponse?> aircrafts, Dictionary<string, AirlineResponse?> airlines, Dictionary<string, AirportResponse?> airports)
    {
        // Mapping aircraft
        foreach (var aircraft in masterData.Aircrafts!)
            if (aircrafts.ContainsKey(aircraft.Code!))
                aircrafts[aircraft.Code!] = aircraft;

        // Mapping airport
        foreach (var airport in masterData.Airports!)
            if (airports.ContainsKey(airport.Code!))
                airports[airport.Code!] = airport;

        // Mapping airline
        foreach (var airline in masterData.Airlines!)
            if (airlines.ContainsKey(airline.Code!))
                airlines[airline.Code!] = airline;

        foreach (var tempSearchDetail in result)
        {
            if (airports.TryGetValue(tempSearchDetail.StartPoint!.Code!, out var tempOne))
                tempSearchDetail.StartPoint = tempOne;
            if (airports.TryGetValue(tempSearchDetail.EndPoint!.Code!, out var tempTwo))
                tempSearchDetail.EndPoint = tempTwo;

            foreach (var tempFlight in tempSearchDetail.ListFlight!)
            {
                if (airlines.TryGetValue(tempFlight.FlightStart!.Airline!.Code!, out var tempThree))
                    tempFlight.FlightStart!.Airline = tempThree;
                if (airlines.TryGetValue(tempFlight.FlightStart!.Operating!.Code!, out var tempFour))
                    tempFlight.FlightStart!.Operating = tempFour;

                if (airlines.TryGetValue(tempFlight?.FlightEnd?.Airline?.Code ?? string.Empty, out var tempFive))
                    tempFlight.FlightEnd.Airline = tempFive;
                if (airlines.TryGetValue(tempFlight?.FlightEnd?.Operating?.Code ?? string.Empty, out var tempSix))
                    tempFlight.FlightEnd.Operating = tempSix;

                if (tempFlight?.FlightStart?.ListFareClass?.Count > 0)
                {
                    foreach (var tempFareClass in tempFlight!.FlightStart!.ListFareClass!)
                    foreach (var tempSegment in tempFareClass!.ListSegment!)
                    {
                        if (airlines.TryGetValue(tempSegment.Airline!.Code!, out var tempSeven))
                            tempSegment.Airline = tempSeven;
                        if (airlines.TryGetValue(tempSegment.Operating!.Code!, out var tempEight))
                            tempSegment.Operating = tempEight;
                        if (airports.TryGetValue(tempSegment.StartPoint!.Code!, out var tempNine))
                            tempSegment.StartPoint = tempNine;
                        if (airports.TryGetValue(tempSegment.EndPoint!.Code!, out var tempTen))
                            tempSegment.EndPoint = tempTen;
                        if (aircrafts.TryGetValue(tempSegment.Plane!.Code!, out var tempEleven))
                            tempSegment.Plane = tempEleven;
                        if (airports.TryGetValue(tempSegment?.StopPoint?.Code ?? string.Empty, out var tempTwelve))
                            tempSegment.StopPoint = tempTwelve;
                    }
                }

                if (tempFlight?.FlightEnd?.ListFareClass?.Count > 0)
                {
                    foreach (var tempFareClass in tempFlight!.FlightEnd!.ListFareClass!)
                    foreach (var tempSegment in tempFareClass!.ListSegment!)
                    {
                        if (airlines.TryGetValue(tempSegment.Airline!.Code!, out var tempSeven))
                            tempSegment.Airline = tempSeven;
                        if (airlines.TryGetValue(tempSegment.Operating!.Code!, out var tempEight))
                            tempSegment.Operating = tempEight;
                        if (airports.TryGetValue(tempSegment.StartPoint!.Code!, out var tempNine))
                            tempSegment.StartPoint = tempNine;
                        if (airports.TryGetValue(tempSegment.EndPoint!.Code!, out var tempTen))
                            tempSegment.EndPoint = tempTen;
                        if (aircrafts.TryGetValue(tempSegment.Plane!.Code!, out var tempEleven))
                            tempSegment.Plane = tempEleven;
                        if (airports.TryGetValue(tempSegment?.StopPoint?.Code ?? string.Empty, out var tempTwelve))
                            tempSegment.StopPoint = tempTwelve;
                    }
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Chức năng: tạo request-payload cho api get-fare-rules
    /// </summary>
    /// <param name="resource"></param>
    /// <returns></returns>
    private static AbTrip.Request.GetFareRulesRequest ComputeGetFareRulesRequest(AbTrip.Response.SearchFlightResponse resource)
    {
        List<AbTrip.Request.FareDataRequest> fareDataRequests = new();

        foreach (var fareData in resource.ListFareData!)
        {
            fareDataRequests.Add(new()
            {
                Session = resource.Session,
                FareDataId = fareData.FareDataId,
                ListFlight = fareData.ListFlight!.Select(x => new AbTrip.Request.FlightRequest()
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

    #region Additional Services

    public async Task<BaseResult<AdditionalServicesResponse>> GetAdditionalServicesAsync(AdditionalServicesRequest request, CancellationToken cancellationToken = default)
    {
        var getAncillaryTask = abTripService.GetAncillaryAsync(mapper.Map<AbTrip.Request.GetAncillaryRequest>(request), cancellationToken);
        var getBaggageTask = abTripService.GetBaggageAsync(mapper.Map<AbTrip.Request.GetBaggageRequest>(request), cancellationToken);

        await Task.WhenAll(getAncillaryTask, getBaggageTask);

        if (getAncillaryTask.Result.CodeMessage == CodeMessage._0000 ||
            getBaggageTask.Result.CodeMessage == CodeMessage._0000)
        {
            var result = ComputeAdditionalServicesResponse(request, getAncillaryTask.Result.Data, getBaggageTask.Result.Data);
            return GetBaseResult(CodeMessage._0000, data: result);
        }

        return GetBaseResult<AdditionalServicesResponse>(CodeMessage._6001);
    }

    private static AdditionalServicesResponse ComputeAdditionalServicesResponse(AdditionalServicesRequest request, AbTrip.Response.GetAncillaryResponse? abTripAncillary, AbTrip.Response.GetBaggageResponse? abTripBaggage)
    {
        // Gom nhóm chiều đi/về
        HashSet<string> keys = new();
        foreach (var fare in request.ListFareData!)
        foreach (var flight in fare.ListFlight!)
            keys.Add($"{flight.StartPoint}-{flight.EndPoint}");

        // Xử lí dữ liệu với từng key
        AdditionalServicesResponse result = new()
        {
            AdditionalServices = new()
        };

        foreach (var key in keys)
        {
            var keySplit = key.Split('-');
            var startPoint = keySplit[0];
            var endPoint = keySplit[1];

            AdditionalServicesInnerResponse inner = new()
            {
                StartPoint = startPoint,
                EndPoint = endPoint,
                ListAncillary = new(),
                ListBaggage = new()
            };

            // Mapping baggage from abtrip
            if (abTripBaggage?.ListBaggage != null && abTripBaggage.ListBaggage.Count > 0)
                foreach (var baggage in abTripBaggage.ListBaggage)
                {
                    if (!string.IsNullOrEmpty(baggage.StartPoint) &&
                        !string.IsNullOrEmpty(baggage.EndPoint) &&
                        baggage.StartPoint.Equals(startPoint, StringComparison.OrdinalIgnoreCase) &&
                        baggage.EndPoint.Equals(endPoint, StringComparison.OrdinalIgnoreCase))
                    {
                        inner.ListBaggage.Add(new()
                        {
                            Airline = baggage.Airline,
                            Leg = baggage.Leg,
                            Route = baggage.Route,
                            Code = baggage.Code,
                            Currency = baggage.Code,
                            Name = baggage.Name,
                            Price = baggage.Price,
                            Value = baggage.Value,
                            Type = baggage.Type,
                            Description = baggage.Description,
                            StartPoint = baggage.StartPoint,
                            EndPoint = baggage.EndPoint,
                            StatusCode = baggage.StatusCode,
                            Confirmed = baggage.Confirmed
                        });
                    }
                }

            // Mapping ancillary from abtrip
            if (abTripAncillary?.ListService != null && abTripAncillary.ListService.Count > 0)
                foreach (var ancillary in abTripAncillary.ListService)
                {
                    if (!string.IsNullOrEmpty(ancillary.StartPoint) &&
                        !string.IsNullOrEmpty(ancillary.EndPoint) &&
                        ancillary.StartPoint.Equals(startPoint, StringComparison.OrdinalIgnoreCase) &&
                        ancillary.EndPoint.Equals(endPoint, StringComparison.OrdinalIgnoreCase))
                    {
                        inner.ListAncillary.Add(new()
                        {
                            Airline = ancillary.Airline,
                            Leg = ancillary.Leg,
                            Route = ancillary.Route,
                            Code = ancillary.Code,
                            Currency = ancillary.Code,
                            Name = ancillary.Name,
                            Price = ancillary.Price,
                            Value = ancillary.Value,
                            Type = ancillary.Type,
                            Description = ancillary.Description,
                            StartPoint = ancillary.StartPoint,
                            EndPoint = ancillary.EndPoint,
                            StatusCode = ancillary.StatusCode,
                            Confirmed = ancillary.Confirmed
                        });
                    }
                }

            result.AdditionalServices.Add(inner);
        }

        return result;
    }

    #endregion

    #region Verify

    public async Task<BaseResult<VerifyResponse>> VerifyAsync(VerifyRequest request, CancellationToken cancellationToken = default)
    {
        var abTripVerify = await abTripService.VerifyFlightAsync(mapper.Map<AbTrip.Request.VerifyFlightRequest>(request), cancellationToken);

        // Process result
        if (abTripVerify.CodeMessage == CodeMessage._0000)
            return GetBaseResult<VerifyResponse>(CodeMessage._0000);

        // Xử lí cho trường hợp thay đổi giá
        if (abTripVerify.CodeMessage == CodeMessage._7004)
        {
            VerifyResponse result = new()
            {
                ListFareData = new()
            };
            int totalPrice = 0;

            foreach (var fareStatus in abTripVerify.Data.ListFareStatus)
            {
                totalPrice += fareStatus.Price ?? 0;
                result.ListFareData.Add(new()
                {
                    Session = fareStatus.Session,
                    FareDataId = fareStatus.FareData.FareDataId,
                    ListFlight = fareStatus.FareData.ListFlight.Select(x => new FlightResponse()
                    {
                        FlightValue = x.FlightValue,
                        StartPoint = x.StartPoint,
                        EndPoint = x.EndPoint
                    }).ToList()
                });
            }

            result.TotalPrice = totalPrice;

            return GetBaseResult(CodeMessage._7004, data: result, message: GetMessageChangePrice(request.TotalPrice, totalPrice));
        }

        // Xử lí cho các trường hợp thất bại
        return GetBaseResult<VerifyResponse>(abTripVerify.CodeMessage);
    }

    private static string GetMessageChangePrice(int? oldPrice, int? newPrice)
    {
        var message = ResponseMessage.Values.TryGetValue(CodeMessage._7004.GetElementNameCodeMessage(), out var value) ? value : string.Empty;

        return message.Replace("[0]", oldPrice.ToString()).Replace("[1]", newPrice.ToString());
    }

    #endregion

    #region Booking

    public async Task<BaseResult<BookingResponse>> BookingAsync(BookingRequest request, CancellationToken cancellationToken = default)
    {
        // Booking to abTrip
        var abTripBookingTask = abTripService.BookFlightAsync(mapper.Map<AbTrip.Request.BookFlightRequest>(request), cancellationToken);
        var getMasterDataTask = GetMasterDataAsync(false, cancellationToken);

        await Task.WhenAll(abTripBookingTask, getMasterDataTask);

        // Process result
        if (abTripBookingTask.Result.CodeMessage == CodeMessage._0000 && getMasterDataTask.Result.CodeMessage == CodeMessage._0000)
        {
            var billModel = await SaveReservationAsync(request, abTripBookingTask.Result.Data!, cancellationToken);
            var result = MappingBookingResponse(billModel, getMasterDataTask.Result.Data!);

            return GetBaseResult(CodeMessage._0000, data: result);
        }

        if (abTripBookingTask.Result.CodeMessage != CodeMessage._0000)
            return GetBaseResult<BookingResponse>(abTripBookingTask.Result.CodeMessage);

        return GetBaseResult<BookingResponse>(getMasterDataTask.Result.CodeMessage);
    }

    /// <summary>
    /// Chức năng: lưu thông tin đặt chỗ vào DB
    /// </summary>
    /// <param name="request"></param>
    /// <param name="abTripBooking"></param>
    /// <param name="cancellationToken"></param>
    private async Task<Model.Bill> SaveReservationAsync(BookingRequest request, AbTrip.Response.BookFlightResponse abTripBooking, CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;
        Model.Bill bill = new()
        {
            IsThirdParty = false,
            PartnerKey = GetPartnerKey(),
            FlightType = MappingFlightType(abTripBooking),
            AbTripOrderId = abTripBooking.OrderId.ToUpperAndRemoveSpace(),
            AbTripBookingId = abTripBooking.BookingId.ToString(),
            AbTripOrderCode = abTripBooking.OrderCode,
            StartTimeZoneOffset = request.StartTimeZoneOffset,
            Contact = mapper.Map<Model.Contact>(request.Contact),
            TotalPrice = abTripBooking.TotalPrice ?? 0,
            Active = true,
            CreatedDatetimeUtc = utcNow,
            UpdatedDatetimeUtc = utcNow,
            Reservations = new(),
            FareDatas = new(),
            FlightDatas = new(),
        };

        // Lưu thông tin invoice
        if (request.Invoice != null)
            bill.Invoice = mapper.Map<Model.Invoice>(request.Invoice);

        // Lấy thông tin booking abtrip
        DateTime? minExpiryDate = null;

        if (abTripBooking.ListBooking != null && abTripBooking.ListBooking.Count > 0)
        {
            foreach (var booking in abTripBooking.ListBooking)
            {
                // Lấy thời gian hết hạn booking theo thời gian nhỏ nhất
                if (minExpiryDate == null)
                    minExpiryDate = booking?.ExpiryDate;
                else if (booking?.ExpiryDate != null && booking.ExpiryDate < minExpiryDate)
                    minExpiryDate = booking.ExpiryDate;

                // Mapping reservation
                bill.Reservations.Add(new()
                {
                    BookingCode = booking?.BookingCode,
                    GdsCode = booking?.GdsCode,
                    ExpiryDate = booking?.ExpiryDate,
                    Airline = booking?.Airline,
                    FlightValue = booking?.Flight,
                    Route = booking?.Route,
                    Session = booking?.Session,
                    Active = true,
                    CreatedDatetimeUtc = utcNow,
                    UpdatedDatetimeUtc = utcNow,
                });

                // Xử lí cho fare-data và flight-data
                if (booking?.ListFareData != null && booking?.ListFareData.Count > 0)
                {
                    foreach (var fare in booking.ListFareData)
                    {
                        // Mapping fare-data
                        bill.FareDatas.Add(new()
                        {
                            BookingCode = booking.BookingCode,
                            AbTripFareDataId = fare?.FareDataId.ToString(),
                            Airline = fare?.Airline,
                            Operating = fare?.System,
                            TotalPrice = fare?.TotalPrice,
                            Adt = fare?.Adt,
                            FareAdt = fare?.FareAdt,
                            TaxAdt = fare?.TaxAdt,
                            FeeAdt = fare?.FeeAdt,
                            ServiceFeeAdt = fare?.ServiceFeeAdt,
                            Chd = fare?.Chd,
                            FareChd = fare?.FareChd,
                            TaxChd = fare?.TaxChd,
                            FeeChd = fare?.FeeChd,
                            ServiceFeeChd = fare?.ServiceFeeChd,
                            Inf = fare?.Inf,
                            FareInf = fare?.FareInf,
                            TaxInf = fare?.TaxInf,
                            FeeInf = fare?.FeeInf,
                            ServiceFeeInf = fare?.ServiceFeeInf,
                            Active = true,
                            CreatedDatetimeUtc = utcNow,
                            UpdatedDatetimeUtc = utcNow,
                        });

                        if (fare?.ListFlight != null && fare.ListFlight.Count > 0)
                            foreach (var flight in fare.ListFlight)
                            {
                                // Mapping flight-data
                                bill.FlightDatas.Add(new()
                                {
                                    AbTripFareDataId = fare?.FareDataId.ToString(),
                                    BookingCode = booking.BookingCode,
                                    FlightId = flight.FlightId.ToString(),
                                    Airline = flight.Airline,
                                    Operating = flight.Operating,
                                    StartPoint = flight.StartPoint,
                                    StartDate = flight.StartDate,
                                    EndPoint = flight.EndPoint,
                                    EndDate = flight.EndDate,
                                    FlightValue = flight.FlightValue,
                                    FlightNumber = flight.FlightNumber,
                                    Active = true,
                                    CreatedDatetimeUtc = utcNow,
                                    UpdatedDatetimeUtc = utcNow,
                                });
                            }
                    }
                }
            }
        }

        // Chuyển đổi thời gian hết hạn booking về UTC
        if (minExpiryDate != null)
        {
            var rawDatetime = $"{minExpiryDate.Value.ConvertToSystemFormat()}{request!.StartTimeZoneOffset}";
            bill.ExpiredDatetimeUtc = DateTimeOffset.Parse(rawDatetime).UtcDateTime;
        }

        // Mapping passenger
        List<Model.Passenger> passengers = new();
        foreach (var passenger in request.ListPassenger!)
        {
            var passengerModel = mapper.Map<Model.Passenger>(passenger);
            var baggages = mapper.Map<List<Model.AdditionalService>>(passenger.ListBaggage, options => options.State = MyEnum.AdditionalServiceType.Baggage);
            var services = mapper.Map<List<Model.AdditionalService>>(passenger.ListService, options => options.State = MyEnum.AdditionalServiceType.Service);
            baggages.AddRange(services);
            passengerModel.AdditionalServices = baggages.ToHashSet();

            passengers.Add(passengerModel);
        }

        // Phân loại điểm khởi hành/kết thúc
        if (bill.FlightDatas != null && bill.FlightDatas.Count > 0)
        {
            if (bill.FlightDatas.Count == 1) // Với chuyến 1 chiều
                bill.FlightDatas.First().Departure = true;
            else if (bill.FlightDatas.Count == 2) // Với chuyến khứ hồi
            {
                var firstFlight = bill.FlightDatas.First();
                var lastFlight = bill.FlightDatas.Last();
                if (firstFlight.StartDate != null && lastFlight.StartDate != null)
                {
                    if (DateTime.Compare(firstFlight.StartDate.Value, lastFlight.StartDate.Value) <= 0)
                        firstFlight.Departure = true;
                    else
                        lastFlight.Departure = true;
                }
            }
        }

        bill.Passengers = passengers.ToHashSet();

        await context.AddAsync(bill, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return bill;
    }

    /// <summary>
    /// Chức năng: xử lí kết quả trả về của booking
    /// </summary>
    /// <param name="bill"></param>
    /// <param name="masterData"></param>
    /// <returns></returns>
    private BookingResponse MappingBookingResponse(Model.Bill bill, MasterDataResponse masterData)
    {
        BookingResponse result = new()
        {
            FlightType = bill.FlightType,
            BillId = bill.Id,
            IsThirdParty = bill.IsThirdParty,
            AbTripOrderId = bill.AbTripOrderId,
            ExpiryDate = bill.ExpiredDatetimeUtc.ConvertUtcToVietnamTz(),
            TotalPrice = bill.TotalPrice,
            OrderDatetimeUtc = bill.CreatedDatetimeUtc,
            Invoice = bill.Invoice != null
                ? new()
                {
                    TaxCode = bill.Invoice.TaxCode,
                    CompanyNameReceive = bill.Invoice.CompanyNameReceive,
                    AddressReceive = bill.Invoice.AddressReceive,
                    CityNameReceive = bill.Invoice.CityNameReceive,
                    ReceiverReceive = bill.Invoice.ReceiverReceive
                }
                : null,
            Contact = bill.Contact != null
                ? new()
                {
                    FirstName = bill.Contact.FirstName,
                    LastName = bill.Contact.LastName,
                    Gender = bill.Contact.Gender,
                    Phone = bill.Contact.Phone,
                    Email = bill.Contact.Email
                }
                : null,
        };

        // Mapping passenger
        if (bill.Passengers != null && bill.Passengers.Count > 0)
        {
            List<PassengerResponse> passengers = new();

            foreach (var passenger in bill.Passengers)
            {
                var tempPassenger = mapper.Map<PassengerResponse>(passenger);
                tempPassenger.ListBaggage = mapper.Map<List<BaggageResponse>>(passenger.AdditionalServices?.Where(x => x.Type == MyEnum.AdditionalServiceType.Baggage));
                tempPassenger.ListService = mapper.Map<List<AncillaryResponse>>(passenger.AdditionalServices?.Where(x => x.Type == MyEnum.AdditionalServiceType.Service));

                passengers.Add(tempPassenger);
            }

            result.ListPassenger = passengers.OrderBy(x => x.Type).ToList();
        }

        // Mapping fare-data
        if (bill.FareDatas != null &&
            bill.FareDatas.Count > 0 &&
            bill.FlightDatas != null &&
            bill.FlightDatas.Count > 0)
        {
            BookingInnerResponse[] fares = new BookingInnerResponse[bill.FlightDatas.Count];

            foreach (var flight in bill.FlightDatas)
            {
                var tempFare = bill.FareDatas.First(x => x.AbTripFareDataId == flight.AbTripFareDataId);
                BookingInnerResponse bookingInnerResponse = new()
                {
                    StartPoint = masterData?.Airports?.Find(x => x.Code!.Equals(flight.StartPoint)),
                    EndPoint = masterData?.Airports?.Find(x => x.Code!.Equals(flight.EndPoint)),
                    StartDate = flight.StartDate,
                    EndDate = flight.EndDate,
                    FareDataId = int.Parse(tempFare.AbTripFareDataId!),
                    Adt = tempFare.Adt,
                    Chd = tempFare.Chd,
                    Inf = tempFare.Inf,
                    UnitPriceAdt = tempFare.FareAdt + tempFare.TaxAdt + tempFare.FeeAdt + tempFare.ServiceFeeAdt,
                    UnitPriceChd = tempFare.FareChd + tempFare.TaxChd + tempFare.FeeChd + tempFare.ServiceFeeChd,
                    UnitPriceInf = tempFare.FareInf + tempFare.TaxInf + tempFare.FeeInf + tempFare.ServiceFeeInf,
                    TotalPrice = tempFare.TotalPrice,
                    FlightNumber = flight.FlightNumber,
                    Airline = masterData?.Airlines?.Find(x => x.Code!.Equals(flight.Airline)),
                    Operating = masterData?.Airlines?.Find(x => x.Code!.Equals(flight.Operating)),
                };

                if (flight.Departure)
                    fares[0] = bookingInnerResponse;
                else
                    fares[1] = bookingInnerResponse;
            }

            result.ListFareData = fares.ToList();
        }

        return result;
    }

    /// <summary>
    /// Chức năng: phân loại flight-type
    /// </summary>
    /// <param name="bookFlightData"></param>
    /// <returns></returns>
    private static MyEnum.FlightType MappingFlightType(AbTrip.Response.BookFlightResponse bookFlightData)
    {
        return bookFlightData.InfoFlight switch
        {
            { FlightType: var x, Itinerary: var y } when x!.Equals("domestic", StringComparison.OrdinalIgnoreCase) && y == 1
                => MyEnum.FlightType.DomesticOneWay,
            { FlightType: var x, Itinerary: var y } when x!.Equals("domestic", StringComparison.OrdinalIgnoreCase) && y == 2
                => MyEnum.FlightType.DomesticRoundTrip,
            { FlightType: var x, Itinerary: var y } when x!.Equals("international", StringComparison.OrdinalIgnoreCase) && y == 1
                => MyEnum.FlightType.InternationalOneWay,
            { FlightType: var x, Itinerary: var y } when x!.Equals("international", StringComparison.OrdinalIgnoreCase) && y == 2
                => MyEnum.FlightType.InternationalRoundTrip,
            _ => MyEnum.FlightType.Other
        };
    }

    #endregion

    #region Issue

    public async Task<BaseResult<IssueResponse>> IssueAsync(IssueRequest request, CancellationToken cancellationToken = default)
    {
        var abTripIssue = await abTripService.IssueAsync(mapper.Map<AbTrip.Request.IssueRequest>(request), cancellationToken);

        // Xử lí dữ liệu trả về
        IssueResponse result = new()
        {
            IssueStatus = new()
        };

        if (abTripIssue?.Data?.Data != null || abTripIssue?.Data?.Data?.Count > 0)
        {
            foreach (var item in abTripIssue.Data.Data)
            {
                if (item.Value == null ||
                    (string.IsNullOrEmpty(item.Value.ErrorCode) && item.Value.Status == null) ||
                    item.Value.ListTicket == null ||
                    item.Value.ListTicket.Count <= 0)
                    continue;

                var firstBooking = item.Value.ListTicket.FirstOrDefault(x => !string.IsNullOrEmpty(x.BookingCode))?.BookingCode;
                var ticketIssued = item.Value?.ErrorCode == "000" && item.Value?.Status == true;
                if (string.IsNullOrEmpty(firstBooking))
                    continue;

                // Trích xuất thông tin ticket
                List<TicketDetailResponse> tickets = new();
                foreach (var ticket in item.Value!.ListTicket)
                {
                    tickets.Add(new()
                    {
                        TicketNumber = ticket.TicketNumber,
                        IssueDatetimeUtc = ticket.IssueDatetime?.ToUniversalTime(),
                        TotalPrice = ticket.TotalPrice,
                        PassengerType = ConvertPassengerType(ticket.PassengerType)
                    });
                }

                result.IssueStatus.TryAdd(firstBooking, new()
                {
                    TicketIssued = ticketIssued,
                    Tickets = tickets
                });
            }
        }

        if (abTripIssue?.CodeMessage == CodeMessage._0000)
        {
            result.AllSuccessful = true;
            return GetBaseResult(CodeMessage._0000, data: result);
        }

        result.AllSuccessful = false;
        return GetBaseResult(CodeMessage._0000, data: result);
    }

    #endregion

    #region Order Info

    public async Task<BaseResult<CheckOrderInfoResponse>> CheckOrderInfoAsync(CheckOrderInfoRequest request, DateTime utcNow, CancellationToken cancellationToken = default)
    {
        // Kiểm tra thông tin giao dịch từ abtrip
        var abTripOrderInfo = await abTripService.OrderInfoAsync(mapper.Map<AbTrip.Request.OrderInfoRequest>(request), cancellationToken);

        if (abTripOrderInfo.CodeMessage != CodeMessage._0000)
            return GetBaseResult<CheckOrderInfoResponse>(CodeMessage._10002);
        if (abTripOrderInfo.Data == null || !abTripOrderInfo.Data.Status!.Value || !abTripOrderInfo.Data.RePayment!.Value)
            return GetBaseResult<CheckOrderInfoResponse>(CodeMessage._10002);

        // Lấy thông tin master-data
        var masterData = await GetMasterDataAsync(false, cancellationToken);
        if (masterData.CodeMessage != CodeMessage._0000)
            return GetBaseResult<CheckOrderInfoResponse>(CodeMessage._3005);

        // Lấy thông tin đơn hàng từ DB
        string abTripOrderId = request.AbTripOrderId.ToUpperAndRemoveSpace();
        var bill = await context.Bills
            .AsNoTracking()
            .Include(x => x.Contact)
            .Include(x => x.Invoice)
            .Include(x => x.FareDatas)
            .Include(x => x.FlightDatas)
            .Include(x => x.PaymentTransactions)
            .Include(x => x.Passengers)!.ThenInclude(y => y.AdditionalServices)
            .FirstOrDefaultAsync(x => x.AbTripOrderId == abTripOrderId && x.ExpiredDatetimeUtc > utcNow, cancellationToken);

        // Kiểm tra giá có sự chênh lệch
        if (bill != null)
        {
            if (abTripOrderInfo.Data.TotalPrice != bill.TotalPrice)
                return GetBaseResult<CheckOrderInfoResponse>(CodeMessage._10003);
            if (bill.PaymentTransactions != null && bill.PaymentTransactions.Any(x => x.PaymentProviderStatus == MyEnum.PaymentStatus.Success))
                return GetBaseResult<CheckOrderInfoResponse>(CodeMessage._10001);

            // Cập nhật lại thông tin partner-key nếu người dùng đang thanh toán trên kênh đối tác
            var partnerKey = GetPartnerKey();
            if (!string.IsNullOrEmpty(partnerKey))
            {
                bill.PartnerKey = partnerKey;
                context.Bills.Update(bill);
                await context.SaveChangesAsync(cancellationToken);
            }

            return GetBaseResult(CodeMessage._0000, data: MappingCheckOrderInfoResponse(bill, masterData.Data!));
        }

        var newBill = await SaveCheckOrderInfoAsync(request, abTripOrderInfo.Data, cancellationToken);
        return GetBaseResult(CodeMessage._0000, data: MappingCheckOrderInfoResponse(newBill, masterData.Data!));
    }

    /// <summary>
    /// Chức năng: lưu thông tin trả sau vào DB
    /// </summary>
    /// <param name="request"></param>
    /// <param name="orderInfo"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="MessageResultException"></exception>
    private async Task<Model.Bill> SaveCheckOrderInfoAsync(CheckOrderInfoRequest request, AbTrip.Response.OrderInfoResponse orderInfo, CancellationToken cancellationToken = default)
    {
        // Validate data
        if (orderInfo.ExpiryDate == null ||
            orderInfo.TotalPrice == null ||
            orderInfo.Contact == null ||
            orderInfo.ListPassenger == null ||
            orderInfo.InfoFlight == null)
            throw new MessageResultException("Dữ liệu order-info không hợp lệ");

        DateTime utcNow = DateTime.UtcNow;
        string? startPoint = orderInfo.InfoFlight.StartPoint;
        string? endPoint = orderInfo.InfoFlight.EndPoint;

        Model.Bill bill = new()
        {
            PartnerKey = GetPartnerKey(),
            FlightType = MappingFlightType(orderInfo),
            IsThirdParty = true,
            AbTripOrderId = request.AbTripOrderId.ToUpperAndRemoveSpace(),
            TotalPrice = orderInfo.TotalPrice.Value,
            Contact = new()
            {
                FirstName = orderInfo.Contact.FirstName,
                LastName = orderInfo.Contact.LastName,
                Gender = orderInfo.Contact.Gender.Value,
                Phone = orderInfo.Contact.Phone,
                Email = orderInfo.Contact.Email,
                Active = true,
                CreatedDatetimeUtc = utcNow,
                UpdatedDatetimeUtc = utcNow
            },
            Active = true,
            CreatedDatetimeUtc = utcNow,
            UpdatedDatetimeUtc = utcNow
        };

        // Mapping ExpiredDatetimeUtc
        var offset = orderInfo?.InfoFlight?.DepartFlight?.ListFlight?.ListSegment?.FirstOrDefault()?.StartTimeZoneOffset;
        if (orderInfo?.ExpiryDate != null && !string.IsNullOrEmpty(offset))
        {
            var rawDatetime = $"{orderInfo.ExpiryDate.Value.ConvertToSystemFormat()}{offset}";
            bill.ExpiredDatetimeUtc = DateTimeOffset.Parse(rawDatetime).UtcDateTime;
        }
        else
        {
            bill.ExpiredDatetimeUtc = utcNow;
        }

        // Mapping Passenger
        if (orderInfo?.ListPassenger.Count > 0)
        {
            HashSet<Model.Passenger> passengers = new();
            foreach (var passenger in orderInfo.ListPassenger)
            {
                var passengerModel = mapper.Map<Model.Passenger>(passenger);

                List<Model.AdditionalService> additionalService = new();

                // Mapping baggage
                if (passenger.ListBaggage != null && passenger.ListBaggage.Count > 0)
                    foreach (var baggage in passenger.ListBaggage)
                    {
                        additionalService.Add(new()
                        {
                            Type = MyEnum.AdditionalServiceType.Baggage,
                            StartPoint = !string.IsNullOrEmpty(baggage?.StartPoint) ? baggage.StartPoint : startPoint,
                            EndPoint = !string.IsNullOrEmpty(baggage?.EndPoint) ? baggage.EndPoint : endPoint,
                            Code = baggage?.Code,
                            Currency = baggage?.Currency,
                            Name = baggage?.Name,
                            Price = baggage?.Price,
                            Value = baggage?.Value,
                        });
                    }

                // Mapping service
                if (passenger.ListService != null && passenger.ListService.Count > 0)
                    foreach (var service in passenger.ListService)
                    {
                        additionalService.Add(new()
                        {
                            Type = MyEnum.AdditionalServiceType.Service,
                            StartPoint = !string.IsNullOrEmpty(service?.StartPoint) ? service.StartPoint : startPoint,
                            EndPoint = !string.IsNullOrEmpty(service?.EndPoint) ? service.EndPoint : endPoint,
                            Code = service?.Code,
                            Currency = service?.Currency,
                            Name = service?.Name,
                            Price = service?.Price,
                            Value = service?.Value,
                        });
                    }

                passengerModel.AdditionalServices = additionalService.ToHashSet();
                passengers.Add(passengerModel);
            }

            bill.Passengers = passengers;
        }

        // Mapping reservations, fareDatas, flightDatas
        HashSet<Model.Reservation>? reservations = new();
        HashSet<Model.FareData>? fareDatas = new();
        HashSet<Model.FlightData>? flightDatas = new();

        int? departFareDataId = null;
        string? departBookingCode = null;
        if (orderInfo.InfoFlight.DepartFlight != null)
        {
            var tempFare = orderInfo.InfoFlight.DepartFlight;

            departFareDataId = tempFare.FareDataId;
            departBookingCode = tempFare.BookingCode;

            // Mapping reservation
            reservations.Add(new()
            {
                BookingCode = tempFare.BookingCode,
                ExpiryDate = orderInfo.ExpiryDate,
                Airline = tempFare.Airline,
                Active = true,
                CreatedDatetimeUtc = utcNow,
                UpdatedDatetimeUtc = utcNow
            });

            // Mapping fare
            fareDatas.Add(new()
            {
                BookingCode = tempFare.BookingCode,
                AbTripFareDataId = tempFare.FareDataId.ToString(),
                Airline = tempFare.Airline,
                Operating = tempFare.System,
                TotalPrice = tempFare.TotalPrice,
                Adt = tempFare.Adt,
                FareAdt = tempFare.FareAdt,
                TaxAdt = tempFare.TaxAdt,
                FeeAdt = tempFare.FeeAdt,
                ServiceFeeAdt = tempFare.ServiceFeeAdt,
                Chd = tempFare.Chd,
                FareChd = tempFare.FareChd,
                TaxChd = tempFare.TaxChd,
                FeeChd = tempFare.FeeChd,
                ServiceFeeChd = tempFare.ServiceFeeChd,
                Inf = tempFare.Inf,
                FareInf = tempFare.FareInf,
                TaxInf = tempFare.TaxInf,
                FeeInf = tempFare.FeeInf,
                ServiceFeeInf = tempFare.ServiceFeeInf,
                Active = true,
                CreatedDatetimeUtc = utcNow,
                UpdatedDatetimeUtc = utcNow
            });

            // Mapping flight
            var tempFlight = tempFare.ListFlight;
            flightDatas.Add(new()
            {
                BookingCode = tempFare.BookingCode,
                AbTripFareDataId = tempFare.FareDataId.ToString(),
                Departure = true,
                Airline = tempFlight?.Airline,
                Operating = tempFlight?.Operating,
                FlightId = tempFlight?.FlightId.ToString(),
                StartPoint = tempFlight?.StartPoint,
                StartDate = tempFlight?.StartDate,
                EndPoint = tempFlight?.EndPoint,
                EndDate = tempFlight?.EndDate,
                FlightValue = tempFlight?.FlightValue,
                FlightNumber = tempFlight?.FlightNumber,
                Active = true,
                CreatedDatetimeUtc = utcNow,
                UpdatedDatetimeUtc = utcNow
            });

            bill.StartTimeZoneOffset = tempFlight?.ListSegment?.FirstOrDefault()?.StartTimeZoneOffset;
        }

        if (orderInfo.InfoFlight.ReturnFlight != null)
        {
            var tempFare = orderInfo.InfoFlight.ReturnFlight;

            var returnFareDataId = tempFare.FareDataId;
            var returnBookingCode = tempFare.BookingCode;

            // Mapping reservation
            if (departBookingCode != null &&
                !departBookingCode.Equals(returnBookingCode, StringComparison.OrdinalIgnoreCase))
            {
                reservations.Add(new()
                {
                    BookingCode = tempFare.BookingCode,
                    ExpiryDate = orderInfo.ExpiryDate,
                    Airline = tempFare.Airline,
                    Active = true,
                    CreatedDatetimeUtc = utcNow,
                    UpdatedDatetimeUtc = utcNow
                });
            }

            // Mapping fare
            if (departFareDataId != null && departFareDataId != returnFareDataId)
            {
                fareDatas.Add(new()
                {
                    BookingCode = tempFare.BookingCode,
                    AbTripFareDataId = tempFare.FareDataId.ToString(),
                    Airline = tempFare.Airline,
                    Operating = tempFare.System,
                    TotalPrice = tempFare.TotalPrice,
                    Adt = tempFare.Adt,
                    FareAdt = tempFare.FareAdt,
                    TaxAdt = tempFare.TaxAdt,
                    FeeAdt = tempFare.FeeAdt,
                    ServiceFeeAdt = tempFare.ServiceFeeAdt,
                    Chd = tempFare.Chd,
                    FareChd = tempFare.FareChd,
                    TaxChd = tempFare.TaxChd,
                    FeeChd = tempFare.FeeChd,
                    ServiceFeeChd = tempFare.ServiceFeeChd,
                    Inf = tempFare.Inf,
                    FareInf = tempFare.FareInf,
                    TaxInf = tempFare.TaxInf,
                    FeeInf = tempFare.FeeInf,
                    ServiceFeeInf = tempFare.ServiceFeeInf,
                    Active = true,
                    CreatedDatetimeUtc = utcNow,
                    UpdatedDatetimeUtc = utcNow
                });
            }

            // Mapping flight
            var tempFlight = tempFare.ListFlight;
            flightDatas.Add(new()
            {
                BookingCode = tempFare.BookingCode,
                AbTripFareDataId = tempFare.FareDataId.ToString(),
                Departure = true,
                Airline = tempFlight?.Airline,
                Operating = tempFlight?.Operating,
                FlightId = tempFlight?.FlightId.ToString(),
                StartPoint = tempFlight?.StartPoint,
                StartDate = tempFlight?.StartDate,
                EndPoint = tempFlight?.EndPoint,
                EndDate = tempFlight?.EndDate,
                FlightValue = tempFlight?.FlightValue,
                FlightNumber = tempFlight?.FlightNumber,
                Active = true,
                CreatedDatetimeUtc = utcNow,
                UpdatedDatetimeUtc = utcNow
            });
        }

        bill.Reservations = reservations;
        bill.FareDatas = fareDatas;
        bill.FlightDatas = flightDatas;

        await context.AddAsync(bill, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return bill;
    }

    /// <summary>
    /// Chức năng: tổ chức dữ liệu trả về cho service
    /// </summary>
    /// <param name="bill"></param>
    /// <param name="masterData"></param>
    /// <returns></returns>
    private CheckOrderInfoResponse MappingCheckOrderInfoResponse(Model.Bill bill, MasterDataResponse masterData)
    {
        CheckOrderInfoResponse result = new()
        {
            FlightType = bill.FlightType,
            BillId = bill.Id,
            ExpiryDate = bill.ExpiredDatetimeUtc.ConvertUtcToVietnamTz(),
            IsThirdParty = bill.IsThirdParty,
            TotalPrice = bill.TotalPrice,
            Invoice = bill.Invoice != null
                ? new()
                {
                    TaxCode = bill.Invoice.TaxCode,
                    CompanyNameReceive = bill.Invoice.CompanyNameReceive,
                    AddressReceive = bill.Invoice.AddressReceive,
                    CityNameReceive = bill.Invoice.CityNameReceive,
                    ReceiverReceive = bill.Invoice.ReceiverReceive
                }
                : null,
            Contact = bill.Contact != null
                ? new()
                {
                    FirstName = bill.Contact.FirstName,
                    LastName = bill.Contact.LastName,
                    Gender = bill.Contact.Gender,
                    Phone = bill.Contact.Phone,
                    Email = bill.Contact.Email
                }
                : null,
        };

        if (bill?.PaymentTransactions != null && bill.PaymentTransactions.Any(x => x.PaymentProviderStatus == MyEnum.PaymentStatus.Success))
            result.IsPaid = true;

        // Mapping passenger
        if (bill.Passengers != null && bill.Passengers.Count > 0)
        {
            List<PassengerResponse> passengers = new();

            foreach (var passenger in bill.Passengers)
            {
                var tempPassenger = mapper.Map<PassengerResponse>(passenger);
                tempPassenger.ListBaggage = mapper.Map<List<BaggageResponse>>(passenger.AdditionalServices?.Where(x => x.Type == MyEnum.AdditionalServiceType.Baggage));
                tempPassenger.ListService = mapper.Map<List<AncillaryResponse>>(passenger.AdditionalServices?.Where(x => x.Type == MyEnum.AdditionalServiceType.Service));

                passengers.Add(tempPassenger);
            }

            result.ListPassenger = passengers.OrderBy(x => x.Type).ToList();
        }

        // Mapping fare-data
        if (bill.FareDatas != null &&
            bill.FareDatas.Count > 0 &&
            bill.FlightDatas != null &&
            bill.FlightDatas.Count > 0)
        {
            List<CheckOrderInfoInnerResponse> fares = new();

            foreach (var flight in bill.FlightDatas)
            {
                var tempFare = bill.FareDatas.First(x => x.AbTripFareDataId == flight.AbTripFareDataId);
                fares.Add(new()
                {
                    StartPoint = masterData?.Airports?.Find(x => x.Code!.Equals(flight.StartPoint)),
                    EndPoint = masterData?.Airports?.Find(x => x.Code!.Equals(flight.EndPoint)),
                    StartDate = flight.StartDate,
                    EndDate = flight.EndDate,
                    FareDataId = int.Parse(tempFare.AbTripFareDataId!),
                    Adt = tempFare.Adt,
                    Chd = tempFare.Chd,
                    Inf = tempFare.Inf,
                    UnitPriceAdt = tempFare.FareAdt + tempFare.TaxAdt + tempFare.FeeAdt + tempFare.ServiceFeeAdt,
                    UnitPriceChd = tempFare.FareChd + tempFare.TaxChd + tempFare.FeeChd + tempFare.ServiceFeeChd,
                    UnitPriceInf = tempFare.FareInf + tempFare.TaxInf + tempFare.FeeInf + tempFare.ServiceFeeInf,
                    TotalPrice = tempFare.TotalPrice,
                    FlightNumber = flight.FlightNumber,
                    Airline = masterData?.Airlines?.Find(x => x.Code!.Equals(flight.Airline)),
                    Operating = masterData?.Airlines?.Find(x => x.Code!.Equals(flight.Operating)),
                });
            }

            result.ListFareData = fares;
        }

        return result;
    }

    /// <summary>
    /// Chức năng: phân loại flight-type
    /// </summary>
    /// <param name="orderInfoData"></param>
    /// <returns></returns>
    private static MyEnum.FlightType MappingFlightType(AbTrip.Response.OrderInfoResponse orderInfoData)
    {
        return orderInfoData.InfoFlight switch
        {
            { FlightType: var x, Itinerary: var y } when x!.Equals("domestic", StringComparison.OrdinalIgnoreCase) && y == 1
                => MyEnum.FlightType.DomesticOneWay,
            { FlightType: var x, Itinerary: var y } when x!.Equals("domestic", StringComparison.OrdinalIgnoreCase) && y == 2
                => MyEnum.FlightType.DomesticRoundTrip,
            { FlightType: var x, Itinerary: var y } when x!.Equals("international", StringComparison.OrdinalIgnoreCase) && y == 1
                => MyEnum.FlightType.InternationalOneWay,
            { FlightType: var x, Itinerary: var y } when x!.Equals("international", StringComparison.OrdinalIgnoreCase) && y == 2
                => MyEnum.FlightType.InternationalRoundTrip,
            _ => MyEnum.FlightType.Other
        };
    }

    #endregion

    #region Convert Ticket Type

    public MyEnum.TicketType ConvertTicketType(MyEnum.FlightType source)
    {
        switch (source)
        {
            case MyEnum.FlightType.DomesticOneWay:
            case MyEnum.FlightType.InternationalOneWay:
                return MyEnum.TicketType.Oneway;
            case MyEnum.FlightType.DomesticRoundTrip:
            case MyEnum.FlightType.InternationalRoundTrip:
                return MyEnum.TicketType.Roundtrip;
            default:
                throw new MessageResultException("Loại chuyến bay không hợp lệ");
        }
    }

    #endregion

    #region Convert Journey Type

    public MyEnum.JourneyType ConvertJourneyType(MyEnum.FlightType source)
    {
        switch (source)
        {
            case MyEnum.FlightType.DomesticOneWay:
            case MyEnum.FlightType.DomesticRoundTrip:
                return MyEnum.JourneyType.Domestic;
            case MyEnum.FlightType.InternationalOneWay:
            case MyEnum.FlightType.InternationalRoundTrip:
                return MyEnum.JourneyType.International;
            default:
                throw new MessageResultException("Loại chuyến bay không hợp lệ");
        }
    }

    #endregion

    #region Private work

    private string? GetPartnerKey()
    {
        if (_httpContext?.Request?.Headers == null)
            return string.Empty;

        foreach (var key in _httpContext.Request.Headers.Keys)
            if (!string.IsNullOrEmpty(key) &&
                key.Equals(SystemConstant.PartnerHeaderKey, StringComparison.OrdinalIgnoreCase) &&
                _httpContext.Request.Headers.TryGetValue(key, out var value))
                return value;

        return string.Empty;
    }

    /// <summary>
    /// Chức năng: chuyển đổi giá trị passenger-type từ abtrip về BE
    /// </summary>
    /// <param name="rawPassengerType"></param>
    /// <returns></returns>
    /// <exception cref="MessageResultException"></exception>
    private static MyEnum.PassengerType? ConvertPassengerType(string? rawPassengerType)
    {
        if (string.IsNullOrEmpty(rawPassengerType))
            return null;

        if (rawPassengerType.Equals("ADT", StringComparison.OrdinalIgnoreCase))
            return MyEnum.PassengerType.ADT;
        if (rawPassengerType.Equals("CHD", StringComparison.OrdinalIgnoreCase))
            return MyEnum.PassengerType.CHD;
        if (rawPassengerType.Equals("INF", StringComparison.OrdinalIgnoreCase))
            return MyEnum.PassengerType.INF;

        throw new MessageResultException("Giá trị passenger-type không hợp lệ");
    }

    private async Task GetConfigDataAsync(CancellationToken cancellationToken = default)
    {
        // Get config from DB
        var configurations = await configurationService.GetAllAsync(false, cancellationToken);

        if (configurations.CodeMessage != CodeMessage._0000)
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