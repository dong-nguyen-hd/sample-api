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
    IMapper mapper,
    CoreContext context) : BaseService, IFlightService
{
    #region Properties

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
    private async Task<List<AirportsResponse>> ComputePopularity(List<AirportsResponse> source)
    {
        List<AirportsResponse> result = new();
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
            if (fareRulesAbTrip.CodeMessage != CodeMessage._0000)
                return GetBaseResult<SearchResponse>(fareRulesAbTrip.CodeMessage);

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

        for (int i = 0; i < searchData.ListFareData!.Count; i++)
        {
            var fare = searchData.ListFareData[i];

            if (fare.ListFlight == null || fare.ListFlight.Count <= 0)
            {
                searchData.ListFareData.RemoveAt(i);
                continue;
            }

            if (flightType == MyEnum.FlightType.InternationalRoundTrip && fare.ListFlight!.Count != 2)
                searchData.ListFareData.RemoveAt(i);
        }

        return searchData;
    }

    /// <summary>
    /// Chức năng: làm sạch dữ liệu fare-rules từ Abtrip
    /// </summary>
    /// <param name="resource"></param>
    /// <returns></returns>
    private static AbTrip.Response.GetFareRulesResponse? CleanFareRulesAbTrip(BaseResult<AbTrip.Response.GetFareRulesResponse> resource)
    {
        if (resource.CodeMessage != CodeMessage._0000 ||
            resource.Data == null ||
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
        Dictionary<string, AircraftsResponse?> aircrafts = new();
        Dictionary<string, AirlinesResponse?> airlines = new();
        Dictionary<string, AirportsResponse?> airports = new();

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
                    FlightValue = flightOne.FlightValue,
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
                    FlightValue = flightTwo.FlightValue,
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
            var tempFareRule = fareRulesData?.ListFareRules!.SingleOrDefault(x => x.FareDataInfo!.FareDataId == fare.FareDataId);

            FareResponse tempFareOne = new()
            {
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
        Dictionary<string, AircraftsResponse?> aircrafts = new();
        Dictionary<string, AirlinesResponse?> airlines = new();
        Dictionary<string, AirportsResponse?> airports = new();

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
                var flight = innerGroup.FareData![0].ListFlight[0];

                // Mapping flight
                airlines.TryAdd(flight.Airline!, default);
                airlines.TryAdd(flight.Operating!, default);
                FilghtDetailResponse filghtDetail = new()
                {
                    FlightStart = new()
                    {
                        Index = index,
                        FlightNumber = innerGroup.FlightNumber,
                        FlightValue = flight.FlightValue,
                        Airline = new()
                        {
                            Code = flight.Airline
                        },
                        Operating = new()
                        {
                            Code = flight.Operating
                        },
                        StartDate = flight.StartDate,
                        EndDate = flight.EndDate,
                        Duration = flight.Duration,
                        StopNum = flight.StopNum,
                        HasUpgradeClass = innerGroup.FareData.Count > 1
                    }
                };

                // Mapping flightStart:listFareClass
                List<FareResponse>? listFare = new();
                foreach (var currentFare in innerGroup.FareData)
                {
                    var tempFareRule = fareRulesData?.ListFareRules!.SingleOrDefault(x => x.FareDataInfo!.FareDataId == currentFare.FareDataId);

                    FareResponse tempFare = new()
                    {
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
    private static List<SearchDetailResponse> MappingMasterDataInFlight(List<SearchDetailResponse> result, MasterDataResponse masterData, Dictionary<string, AircraftsResponse?> aircrafts, Dictionary<string, AirlinesResponse?> airlines, Dictionary<string, AirportsResponse?> airports)
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

        if (getAncillaryTask.Result.CodeMessage != CodeMessage._0000)
            return GetBaseResult<AdditionalServicesResponse>(getAncillaryTask.Result.CodeMessage);

        return GetBaseResult<AdditionalServicesResponse>(getBaggageTask.Result.CodeMessage);
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
            var result = MappingBookingResponse(request, abTripBookingTask.Result.Data!, getMasterDataTask.Result.Data!);
            result.BillId = billModel.Id;

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
            IsPaylater = false,
            AbTripOrderId = "none",
            AbTripBookingId = abTripBooking.BookingId.ToString(),
            AbTripOrderCode = abTripBooking.OrderCode,
            Contact = mapper.Map<Model.Contact>(request.Contact),
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
        List<Model.Reservation> reservations = new();
        DateTime? minExpiryDate = null;
        int totalPrice = 0;

        if (abTripBooking.ListBooking != null && abTripBooking.ListBooking.Count > 0)
        {
            // Xác định ticket-type
            if (abTripBooking.ListBooking.Count <= 1)
            {
                var firstBooking = abTripBooking.ListBooking[0];
                if (firstBooking?.Flight?.Contains('|') ?? false)
                    bill.TicketType = MyEnum.TicketType.Roundtrip;
                else
                    bill.TicketType = MyEnum.TicketType.Oneway;
            }
            else
                bill.TicketType = MyEnum.TicketType.Roundtrip;

            foreach (var booking in abTripBooking.ListBooking)
            {
                totalPrice += booking?.Price ?? 0;

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
                                    FlightId = flight.FlightId.ToString(),
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

        bill.TotalPrice = totalPrice;
        bill.Passengers = passengers.ToHashSet();

        await context.AddAsync(bill, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return bill;
    }

    /// <summary>
    /// Chức năng: xử lí kết quả trả về của booking
    /// </summary>
    /// <param name="request"></param>
    /// <param name="abTripBooking"></param>
    /// <param name="masterData"></param>
    /// <returns></returns>
    private BookingResponse MappingBookingResponse(BookingRequest request, AbTrip.Response.BookFlightResponse abTripBooking, MasterDataResponse masterData)
    {
        BookingResponse result = new()
        {
            IsPaylater = false,
            Invoice = new()
            {
                TaxCode = request?.Invoice?.TaxCode,
                CompanyNameReceive = request?.Invoice?.CompanyNameReceive,
                AddressReceive = request?.Invoice?.AddressReceive,
                CityNameReceive = request?.Invoice?.CityNameReceive,
                ReceiverReceive = request?.Invoice?.ReceiverReceive
            },
            Contact = new()
            {
                FirstName = request?.Contact?.FirstName,
                LastName = request?.Contact?.LastName,
                Gender = (bool)request?.Contact?.Gender,
                Phone = request?.Contact?.Phone,
                Email = request?.Contact?.Email,
            }
        };

        List<PassengerResponse> passengers = new();
        List<BookingInnerResponse> fares = new();
        bool hasPassenger = false;
        int totalPrice = 0;

        if (abTripBooking.ListBooking != null && abTripBooking.ListBooking.Count > 0)
        {
            DateTime? minExpiryDate = null;

            foreach (var booking in abTripBooking.ListBooking)
            {
                // Lấy thời gian hết hạn booking theo thời gian nhỏ nhất
                if (minExpiryDate == null)
                    minExpiryDate = booking.ExpiryDate;
                else if (booking.ExpiryDate != null && booking.ExpiryDate < minExpiryDate)
                    minExpiryDate = booking.ExpiryDate;

                totalPrice += booking.Price ?? 0;

                if (!hasPassenger)
                {
                    passengers = mapper.Map<List<PassengerResponse>>(booking.ListPassenger);
                    hasPassenger = true;
                }

                foreach (var fare in booking.ListFareData!)
                foreach (var flight in fare.ListFlight!)
                    fares.Add(new()
                    {
                        StartPoint = masterData.Airports!.Find(x => x.Code!.Equals(flight.StartPoint)),
                        EndPoint = masterData.Airports.Find(x => x.Code!.Equals(flight.EndPoint)),
                        StartDate = flight.StartDate,
                        EndDate = flight.EndDate,
                        FareDataId = fare.FareDataId,
                        Adt = fare.Adt,
                        Chd = fare.Chd,
                        Inf = fare.Inf,
                        UnitPriceAdt = fare.FareAdt + fare.TaxAdt + fare.FeeAdt + fare.ServiceFeeAdt,
                        UnitPriceChd = fare.FareChd + fare.TaxChd + fare.FeeChd + fare.ServiceFeeChd,
                        UnitPriceInf = fare.FareInf + fare.TaxInf + fare.FeeInf + fare.ServiceFeeInf,
                        TotalPrice = fare.TotalPrice,
                        FlightNumber = flight.FlightNumber
                    });
            }

            // Chuyển đổi thời gian hết hạn booking về +7:00
            if (minExpiryDate != null)
            {
                var rawDatetime = $"{minExpiryDate.Value.ConvertToSystemFormat()}{request!.StartTimeZoneOffset}";
                var utcTime = DateTimeOffset.Parse(rawDatetime).UtcDateTime;
                result.ExpiryDate = utcTime.ConvertUtcToVietnamTz();
            }
        }

        result.ListPassenger = passengers;
        result.ListFareData = fares;
        result.TotalPrice = totalPrice;
        return result;
    }

    #endregion

    #region Private work

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