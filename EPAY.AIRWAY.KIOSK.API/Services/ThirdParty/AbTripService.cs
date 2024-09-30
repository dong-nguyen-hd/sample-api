using System.Collections;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.CustomHttpClient.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip.Response;
using EPAY.AIRWAY.KIOSK.API.Resources.Exceptions;
using EPAY.AIRWAY.KIOSK.API.Resources.SystemData.ThirdParty.AbTrip;

namespace EPAY.AIRWAY.KIOSK.API.Services.ThirdParty;

public class AbTripService(
    IConfigurationService configurationService,
    ICustomHttpClient customHttpClient) : BaseService, IAbTripService
{
    #region Properties

    private AbTripInfo? _abTripInfo;

    #endregion

    #region Method

    public async Task<BaseResult<List<AircraftsResponse>>> GetAircraftsAsync(CancellationToken cancellationToken = default)
    {
        // Get config
        var info = await GetConfigDataAsync(cancellationToken);

        // Request to 3th
        AircraftsRequest payload = new()
        {
            Username = info.Config.Username,
            Password = info.Config.Password
        };
        var baseResponse = await customHttpClient.SendAsync(new MyHttpRequest
        {
            Uri = new Uri(info.Api.GetAircraftsUri()),
            Payload = payload.MySerialize(),
            MyHttpMethod = MyEnum.MyHttpMethod.GET,
            NumberRetry = 2,
            EnableVerifyTls = info.Api.EnableVerifyTls
        }, ProcessResult<List<AircraftsResponse>>, CodeMessage._0009, cancellationToken);

        return GetBaseResult(baseResponse.codeMessage, baseResponse.data);
    }

    public async Task<BaseResult<List<AirlinesResponse>>> GetAirlinesAsync(CancellationToken cancellationToken = default)
    {
        // Get config
        var info = await GetConfigDataAsync(cancellationToken);

        // Request to 3th
        AirlinesRequest payload = new()
        {
            Username = info.Config.Username,
            Password = info.Config.Password
        };
        var baseResponse = await customHttpClient.SendAsync(new MyHttpRequest
        {
            Uri = new Uri(info.Api.GetAirlinesUri()),
            Payload = payload.MySerialize(),
            MyHttpMethod = MyEnum.MyHttpMethod.GET,
            NumberRetry = 2,
            EnableVerifyTls = info.Api.EnableVerifyTls
        }, ProcessResult<List<AirlinesResponse>>, CodeMessage._0009, cancellationToken);

        return GetBaseResult(baseResponse.codeMessage, baseResponse.data);
    }

    public async Task<BaseResult<List<AirportsResponse>>> GetAirportsAsync(CancellationToken cancellationToken = default)
    {
        // Get config
        var info = await GetConfigDataAsync(cancellationToken);

        // Request to 3th
        AirportsRequest payload = new()
        {
            Username = info.Config.Username,
            Password = info.Config.Password
        };
        var baseResponse = await customHttpClient.SendAsync(new MyHttpRequest
        {
            Uri = new Uri(info.Api.GetAirportsUri()),
            Payload = payload.MySerialize(),
            MyHttpMethod = MyEnum.MyHttpMethod.GET,
            NumberRetry = 2,
            EnableVerifyTls = info.Api.EnableVerifyTls
        }, ProcessResult<List<AirportsResponse>>, CodeMessage._0009, cancellationToken);

        return GetBaseResult(baseResponse.codeMessage, baseResponse.data);
    }

    public async Task<BaseResult<BookFlightResponse>> BookFlightAsync(BookFlightRequest request, CancellationToken cancellationToken = default)
    {
        // Get config
        var info = await GetConfigDataAsync(cancellationToken);

        // Request to 3th
        request.Username = info.Config!.Username;
        request.Password = info.Config.Password;

        var baseResponse = await customHttpClient.SendAsync(new MyHttpRequest
        {
            Uri = new Uri(info.Api!.GetBookFlightUri()),
            Payload = request.MySerialize(),
            MyHttpMethod = MyEnum.MyHttpMethod.POST,
            NumberRetry = 0,
            EnableVerifyTls = info.Api.EnableVerifyTls
        }, ProcessResult<BookFlightResponse>, CodeMessage._0009, cancellationToken);

        return GetBaseResult(baseResponse.codeMessage, baseResponse.data);
    }

    public async Task<BaseResult<GetFareRulesResponse>> GetFareRulesAsync(GetFareRulesRequest request, CancellationToken cancellationToken = default)
    {
        // Get config
        var info = await GetConfigDataAsync(cancellationToken);

        // Request to 3th
        request.Username = info.Config!.Username;
        request.Password = info.Config.Password;

        var baseResponse = await customHttpClient.SendAsync(new MyHttpRequest
        {
            Uri = new Uri(info.Api!.GetFareRulesUri()),
            Payload = request.MySerialize(),
            MyHttpMethod = MyEnum.MyHttpMethod.POST,
            NumberRetry = 2,
            EnableVerifyTls = info.Api.EnableVerifyTls
        }, ProcessResult<GetFareRulesResponse>, CodeMessage._0009, cancellationToken);

        return GetBaseResult(baseResponse.codeMessage, baseResponse.data);
    }

    public async Task<BaseResult<PriceQuoteResponse>> PriceQuoteAsync(PriceQuoteRequest request, CancellationToken cancellationToken = default)
    {
        // Get config
        var info = await GetConfigDataAsync(cancellationToken);

        // Request to 3th
        request.Username = info.Config!.Username;
        request.Password = info.Config.Password;

        var baseResponse = await customHttpClient.SendAsync(new MyHttpRequest
        {
            Uri = new Uri(info.Api!.GetPriceQuoteUri()),
            Payload = request.MySerialize(),
            MyHttpMethod = MyEnum.MyHttpMethod.POST,
            NumberRetry = 0,
            EnableVerifyTls = info.Api.EnableVerifyTls
        }, ProcessResult<PriceQuoteResponse>, CodeMessage._0009, cancellationToken);

        return GetBaseResult(baseResponse.codeMessage, baseResponse.data);
    }

    public async Task<BaseResult<SearchFlightResponse>> SearchFlightAsync(SearchFlightRequest request, CancellationToken cancellationToken = default)
    {
        // Get config
        var info = await GetConfigDataAsync(cancellationToken);

        // Request to 3th
        request.Username = info.Config!.Username;
        request.Password = info.Config.Password;

        var baseResponse = await customHttpClient.SendAsync(new MyHttpRequest
        {
            Uri = new Uri(info.Api!.GetSearchFlightUri()),
            Payload = request.MySerialize(),
            MyHttpMethod = MyEnum.MyHttpMethod.POST,
            NumberRetry = 2,
            EnableVerifyTls = info.Api.EnableVerifyTls
        }, ProcessResult<SearchFlightResponse>, CodeMessage._0009, cancellationToken);

        return GetBaseResult(baseResponse.codeMessage, baseResponse.data);
    }

    public async Task<BaseResult<VerifyFlightResponse>> VerifyFlightAsync(VerifyFlightRequest request, CancellationToken cancellationToken = default)
    {
        // Get config
        var info = await GetConfigDataAsync(cancellationToken);

        // Request to 3th
        request.Username = info.Config!.Username;
        request.Password = info.Config.Password;

        var baseResponse = await customHttpClient.SendAsync(new MyHttpRequest
        {
            Uri = new Uri(info.Api!.GetVerifyFlightUri()),
            Payload = request.MySerialize(),
            MyHttpMethod = MyEnum.MyHttpMethod.POST,
            NumberRetry = 2,
            EnableVerifyTls = info.Api.EnableVerifyTls
        }, ProcessResult<VerifyFlightResponse>, CodeMessage._0009, cancellationToken);

        return GetBaseResult(baseResponse.codeMessage, baseResponse.data);
    }

    public async Task<BaseResult<GetBaggageResponse>> GetBaggageAsync(GetBaggageRequest request, CancellationToken cancellationToken = default)
    {
        // Get config
        var info = await GetConfigDataAsync(cancellationToken);

        // Request to 3th
        request.Username = info.Config!.Username;
        request.Password = info.Config.Password;

        var baseResponse = await customHttpClient.SendAsync(new MyHttpRequest
        {
            Uri = new Uri(info.Api!.GetBaggageUri()),
            Payload = request.MySerialize(),
            MyHttpMethod = MyEnum.MyHttpMethod.POST,
            NumberRetry = 2,
            EnableVerifyTls = info.Api.EnableVerifyTls
        }, ProcessResult<GetBaggageResponse>, CodeMessage._0009, cancellationToken);

        return GetBaseResult(baseResponse.codeMessage, baseResponse.data);
    }

    public async Task<BaseResult<GetAncillaryResponse>> GetAncillaryAsync(GetAncillaryRequest request, CancellationToken cancellationToken = default)
    {
        // Get config
        var info = await GetConfigDataAsync(cancellationToken);

        // Request to 3th
        request.Username = info.Config!.Username;
        request.Password = info.Config.Password;

        var baseResponse = await customHttpClient.SendAsync(new MyHttpRequest
        {
            Uri = new Uri(info.Api!.GetAncillaryUri()),
            Payload = request.MySerialize(),
            MyHttpMethod = MyEnum.MyHttpMethod.POST,
            NumberRetry = 2,
            EnableVerifyTls = info.Api.EnableVerifyTls
        }, ProcessResult<GetAncillaryResponse>, CodeMessage._0009, cancellationToken);

        return GetBaseResult(baseResponse.codeMessage, baseResponse.data);
    }

    public async Task<BaseResult<IssueResponse>> IssueAsync(IssueRequest request, CancellationToken cancellationToken = default)
    {
        // Get config
        var info = await GetConfigDataAsync(cancellationToken);

        // Request to 3th
        request.Username = info.Config!.Username;
        request.Password = info.Config.Password;

        var baseResponse = await customHttpClient.SendAsync(new MyHttpRequest
        {
            Uri = new Uri(info.Api!.GetIssueUri()),
            Payload = request.MySerialize(),
            MyHttpMethod = MyEnum.MyHttpMethod.POST,
            NumberRetry = 0,
            EnableVerifyTls = info.Api.EnableVerifyTls
        }, ProcessResult<IssueResponse>, CodeMessage._0009, cancellationToken);

        return GetBaseResult(baseResponse.codeMessage, baseResponse.data);
    }
    
    public async Task<BaseResult<OrderInfoResponse>> OrderInfoAsync(OrderInfoRequest request, CancellationToken cancellationToken = default)
    {
        // Get config
        var info = await GetConfigDataAsync(cancellationToken);

        // Request to 3th
        request.Username = info.Config!.Username;
        request.Password = info.Config.Password;

        var baseResponse = await customHttpClient.SendAsync(new MyHttpRequest
        {
            Uri = new Uri(info.Api!.GetOrderInfoUri()),
            Payload = request.MySerialize(),
            MyHttpMethod = MyEnum.MyHttpMethod.GET,
            NumberRetry = 2,
            EnableVerifyTls = info.Api.EnableVerifyTls
        }, ProcessResult<OrderInfoResponse>, CodeMessage._0009, cancellationToken);

        return GetBaseResult(baseResponse.codeMessage, baseResponse.data);
    }
    
    public async Task<AbTripInfo> GetConfigDataAsync(CancellationToken cancellationToken = default)
    {
        // Sử dụng lại config đã lấy ra trước đó nếu có dữ liệu
        if (_abTripInfo != null)
            return _abTripInfo with { };

        // Get config from DB
        var configurations = await configurationService.GetAllAsync(false, cancellationToken);

        if (configurations.CodeMessage != CodeMessage._0000)
            throw new MessageResultException("Không thể thực hiện lấy config");

        // Process result
        AbTripInfo info = new()
        {
            Config = new(),
            Api = new()
        };

        foreach (var configuration in configurations.Data)
        {
            // Config
            if (configuration.Key == SystemConfig.AbTripUsername)
            {
                info.Config.Username = configuration.Value;
                continue;
            }

            if (configuration.Key == SystemConfig.AbTripPassword)
            {
                info.Config.Password = configuration.Value.MyAesDecrypt(ThirdPartyEncryption.Secret);
                continue;
            }

            // Api
            if (configuration.Key == SystemConfig.AbTripEnableVerifyTls)
            {
                info.Api.EnableVerifyTls = bool.Parse(configuration.Value);
                continue;
            }

            if (configuration.Key == SystemConfig.AbTripBaseAddress)
            {
                info.Api.BaseAddress = configuration.Value;
                continue;
            }

            if (configuration.Key == SystemConfig.AbTripSearchFlight)
            {
                info.Api.SearchFlight = configuration.Value;
                continue;
            }

            if (configuration.Key == SystemConfig.AbTripBaggage)
            {
                info.Api.Baggage = configuration.Value;
                continue;
            }

            if (configuration.Key == SystemConfig.AbTripAncillary)
            {
                info.Api.Ancillary = configuration.Value;
                continue;
            }

            if (configuration.Key == SystemConfig.AbTripFareRules)
            {
                info.Api.FareRules = configuration.Value;
                continue;
            }

            if (configuration.Key == SystemConfig.AbTripVerifyFlight)
            {
                info.Api.VerifyFlight = configuration.Value;
                continue;
            }

            if (configuration.Key == SystemConfig.AbTripPriceQuote)
            {
                info.Api.PriceQuote = configuration.Value;
                continue;
            }

            if (configuration.Key == SystemConfig.AbTripBookFlight)
            {
                info.Api.BookFlight = configuration.Value;
                continue;
            }

            if (configuration.Key == SystemConfig.AbTripAircrafts)
            {
                info.Api.Aircrafts = configuration.Value;
                continue;
            }

            if (configuration.Key == SystemConfig.AbTripAirports)
            {
                info.Api.Airports = configuration.Value;
                continue;
            }

            if (configuration.Key == SystemConfig.AbTripAirlines)
            {
                info.Api.Airlines = configuration.Value;
                continue;
            }
            
            if (configuration.Key == SystemConfig.AbTripIssue)
            {
                info.Api.Issue = configuration.Value;
                continue;
            }
            
            if (configuration.Key == SystemConfig.AbTripOrderInfo)
            {
                info.Api.OrderInfo = configuration.Value;
                continue;
            }
        }

        _abTripInfo = info;

        return info;
    }

    #region Private work

    private static (CodeMessage, TRes?) ProcessResult<TRes>(HttpResponseMessage resource, string rawPayload)
    {
        if (!string.IsNullOrEmpty(rawPayload))
        {
            var result = JsonSerializer.Deserialize<TRes>(rawPayload);

            if (result == null)
                return (CodeMessage._0009, default);

            if (result is BaseResponse parsed)
            {
                if (parsed.ErrorCode == "000")
                    return (CodeMessage._0000, result);
                if (parsed.ErrorCode == "0032")
                    return (CodeMessage._7004, result);

                switch (parsed.ErrorCode)
                {
                    case "000":
                        return (CodeMessage._0000, result);
                    case "001":
                    case "201":
                    case "202":
                    case "203":
                    case "204":
                    case "205":
                    case "206":
                    case "207":
                    case "208":
                    case "209":
                    case "210":
                    case "216":
                    case "228":
                    case "318":
                    case "313":
                    case "227":
                    case "0044":
                    case "0048":
                        return (CodeMessage._0001, result);
                    case "002":
                    case "0003":
                    case "0042":
                        return (CodeMessage._0002, result);
                    case "101":
                    case "102":
                    case "103":
                    case "232":
                    case "105":
                    case "106":
                    case "107":
                    case "108":
                    case "109":
                    case "321":
                    case "0004":
                    case "0028":
                    case "0040":
                    case "0401":
                        return (CodeMessage._0003, result);
                    case "301":
                    case "0029":
                    case "0041":
                    case "0050":
                        return (CodeMessage._0004, result);
                    case "302":
                        return (CodeMessage._0005, result);
                    case "003":
                        return (CodeMessage._0006, result);
                    case "226":
                        return (CodeMessage._0007, result);
                    case "004":
                        return (CodeMessage._0008, result);
                    case "316":
                    case "229":
                        return (CodeMessage._5001, result);
                    case "320":
                        return (CodeMessage._5002, result);
                    case "231":
                        return (CodeMessage._5003, result);
                    case "319":
                        return (CodeMessage._6001, result);
                    case "089":
                        return (CodeMessage._6002, result);
                    case "0006":
                        return (CodeMessage._7001, result);
                    case "0008":
                    case "0034":
                        return (CodeMessage._7002, result);
                    case "0015":
                        return (CodeMessage._7003, result);
                    case "0032":
                    case "309":
                        return (CodeMessage._7004, result);
                    case "0002":
                    case "0005":
                    case "0007":
                    case "0010":
                    case "0011":
                    case "0012":
                    case "0014":
                    case "0017":
                    case "0018":
                    case "0020":
                    case "0024":
                    case "0025":
                    case "0030":
                    case "0033":
                    case "0035":
                    case "0043":
                    case "0045":
                    case "0046":
                    case "0047":
                    case "0051":
                        return (CodeMessage._8001, result);
                    default:
                        return (CodeMessage._0009, result);
                }
            }

            if (result is IList { Count: > 0 })
                return (CodeMessage._0000, result);
        }

        return (CodeMessage._0009, default);
    }

    #endregion

    #endregion
}