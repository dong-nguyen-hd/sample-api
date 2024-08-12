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
        {
            cacheData.Popularity = await ComputePopularity(cacheData.Airports!);
            return GetBaseResult(CodeMessage._99, data: cacheData);
        }

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

            resultInner.Popularity = await ComputePopularity(resultInner.Airports!);
            return GetBaseResult(CodeMessage._99, data: resultInner);
        }

        return GetBaseResult<MasterDataResponse>(CodeMessage._100);
    }

    private async Task<List<AirportsResponse>> ComputePopularity(List<AirportsResponse> source)
    {
        List<AirportsResponse> result = new();
        
        // TODO: bổ sung phần cơ chế tính động
        foreach (var airport in source)
        {
            if(airport.Code == "HAN")
                result.Add(airport);
            if(airport.Code == "SGN")
                result.Add(airport);
            if(airport.Code == "DAD")
                result.Add(airport);
            if(airport.Code == "CXR")
                result.Add(airport);
        }

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
        // TH1: Với chuyến bay nội địa 1-2 chiều, quốc tế 1 chiều => gộp theo điều kiện flight-number và start-date
        // TH2: Với chuyến bay quốc tế 2 chiều => gộp theo fare-data-id

        Dictionary<string, AircraftsResponse?> aircrafts = new();
        Dictionary<string, AirlinesResponse?> airlines = new();
        Dictionary<string, AirportsResponse?> airports = new();

        if (result.FlightType == MyEnum.FlightType.InternationalTwoWay)
        {
            int index = 0;

            // Duyệt qua từng fare (abtrip)
            foreach (var fare in searchData.ListFareData!)
            {
                // Lọc dữ liệu chỉ chứa 2 chiều bay
                if (fare.ListFlight.Count != 2)
                    continue;

                AbTrip.Response.FlightResponse flightOne;
                AbTrip.Response.FlightResponse flightTwo;
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
                    UnitPriceAdt = 10000,
                    UnitPriceChd = 11000,
                    UnitPriceInf = 120000,
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
                    UnitPriceAdt = 10000,
                    UnitPriceChd = 11000,
                    UnitPriceInf = 120000,
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

                tempFareOne.ListSegment = listFlightSegmentOne;
                tempFareTwo.ListSegment = listFlightSegmentTwo;
                listFareOne.Add(tempFareOne);
                listFareTwo.Add(tempFareTwo);

                filghtDetail.FlightStart.ListFareClass = listFareOne;
                filghtDetail.FlightEnd.ListFareClass = listFareTwo;
                listFlightDetail.Add(filghtDetail);

                searchDetail.ListFlight = listFlightDetail;
                result.SearchDetail.Add(searchDetail);

                index++;
            }
        }
        else
        {
            // Gom nhóm dữ liệu
            List<GroupDataRequest> bucket = new();

            // Duyệt qua từng fare (abtrip)
            foreach (var fare in searchData.ListFareData)
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
                            UnitPriceAdt = 10000,
                            UnitPriceChd = 11000,
                            UnitPriceInf = 120000,
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
                result.SearchDetail.Add(searchDetail);
            }
        }

        // Mapping master-data
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

        foreach (var tempSearchDetail in result.SearchDetail)
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

                foreach (var tempFareClass in tempFlight!.FlightStart!.ListFareClass!)
                foreach (var tempSegment in tempFareClass!.ListSegment!)
                {
                    if (airlines.TryGetValue(tempSegment.Airline!.Code!, out var tempFive))
                        tempSegment.Airline = tempFive;
                    if (airlines.TryGetValue(tempSegment.Operating!.Code!, out var tempSix))
                        tempSegment.Operating = tempSix;
                    if (airports.TryGetValue(tempSegment.StartPoint!.Code!, out var tempSeven))
                        tempSegment.StartPoint = tempSeven;
                    if (airports.TryGetValue(tempSegment.EndPoint!.Code!, out var tempEight))
                        tempSegment.EndPoint = tempEight;
                    if (aircrafts.TryGetValue(tempSegment.Plane!.Code!, out var tempNine))
                        tempSegment.Plane = tempNine;
                }
            }
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