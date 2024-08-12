using EPAY.AIRWAY.KIOSK.API.Domain.Context;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.CustomHttpClient.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip.Response;
using EPAY.AIRWAY.KIOSK.API.Resources.Enums;
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
        var baseResponse = await customHttpClient.SendAsync<List<AircraftsResponse>>(new MyHttpRequest
        {
            Uri = new Uri(info.Api.GetAircraftsUri()),
            Payload = payload.MySerialize(),
            MyHttpMethod = MyHttpMethod.GET,
            NumberRetry = 2,
            EnableVerifyTls = info.Api.EnableVerifyTls
        }, cancellationToken);

        // Process result
        if (!baseResponse.isSuccess)
            return GetBaseResult<List<AircraftsResponse>>(CodeMessage._100);

        return GetBaseResult(CodeMessage._0000, baseResponse.data);
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
        var baseResponse = await customHttpClient.SendAsync<List<AirlinesResponse>>(new MyHttpRequest
        {
            Uri = new Uri(info.Api.GetAirlinesUri()),
            Payload = payload.MySerialize(),
            MyHttpMethod = MyHttpMethod.GET,
            NumberRetry = 2,
            EnableVerifyTls = info.Api.EnableVerifyTls
        }, cancellationToken);

        // Process result
        if (!baseResponse.isSuccess)
            return GetBaseResult<List<AirlinesResponse>>(CodeMessage._100);

        return GetBaseResult(CodeMessage._0000, baseResponse.data);
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
        var baseResponse = await customHttpClient.SendAsync<List<AirportsResponse>>(new MyHttpRequest
        {
            Uri = new Uri(info.Api.GetAirportsUri()),
            Payload = payload.MySerialize(),
            MyHttpMethod = MyHttpMethod.GET,
            NumberRetry = 2,
            EnableVerifyTls = info.Api.EnableVerifyTls
        }, cancellationToken);

        // Process result
        if (!baseResponse.isSuccess)
            return GetBaseResult<List<AirportsResponse>>(CodeMessage._100);

        return GetBaseResult(CodeMessage._0000, baseResponse.data);
    }

    public async Task<BaseResult<BookFlightResponse>> BookFlightAsync(BookFlightRequest request, CancellationToken cancellationToken = default)
    {
        // Get config
        var info = await GetConfigDataAsync(cancellationToken);

        // Request to 3th
        request.Username = info.Config!.Username;
        request.Password = info.Config.Password;

        var baseResponse = await customHttpClient.SendAsync<BookFlightResponse>(new MyHttpRequest
        {
            Uri = new Uri(info.Api!.GetBookFlightUri()),
            Payload = request.MySerialize(),
            MyHttpMethod = MyHttpMethod.POST,
            NumberRetry = 0,
            EnableVerifyTls = info.Api.EnableVerifyTls
        }, cancellationToken);

        // Process result
        if (!baseResponse.isSuccess)
            return GetBaseResult<BookFlightResponse>(CodeMessage._100);

        return GetBaseResult(CodeMessage._0000, baseResponse.data);
    }

    public async Task<BaseResult<GetFareRulesResponse>> GetFareRulesAsync(GetFareRulesRequest request, CancellationToken cancellationToken = default)
    {
        // Get config
        var info = await GetConfigDataAsync(cancellationToken);

        // Request to 3th
        request.Username = info.Config!.Username;
        request.Password = info.Config.Password;

        var baseResponse = await customHttpClient.SendAsync<GetFareRulesResponse>(new MyHttpRequest
        {
            Uri = new Uri(info.Api!.GetFareRulesUri()),
            Payload = request.MySerialize(),
            MyHttpMethod = MyHttpMethod.POST,
            NumberRetry = 2,
            EnableVerifyTls = info.Api.EnableVerifyTls
        }, cancellationToken);

        // Process result
        if (!baseResponse.isSuccess)
            return GetBaseResult<GetFareRulesResponse>(CodeMessage._100);

        return GetBaseResult(CodeMessage._0000, baseResponse.data);
    }
    
    public async Task<BaseResult<PriceQuoteResponse>> PriceQuoteAsync(PriceQuoteRequest request, CancellationToken cancellationToken = default)
    {
        // Get config
        var info = await GetConfigDataAsync(cancellationToken);

        // Request to 3th
        request.Username = info.Config!.Username;
        request.Password = info.Config.Password;

        var baseResponse = await customHttpClient.SendAsync<PriceQuoteResponse>(new MyHttpRequest
        {
            Uri = new Uri(info.Api!.GetPriceQuoteUri()),
            Payload = request.MySerialize(),
            MyHttpMethod = MyHttpMethod.POST,
            NumberRetry = 0,
            EnableVerifyTls = info.Api.EnableVerifyTls
        }, cancellationToken);

        // Process result
        if (!baseResponse.isSuccess)
            return GetBaseResult<PriceQuoteResponse>(CodeMessage._100);

        return GetBaseResult(CodeMessage._0000, baseResponse.data);
    }
    
    public async Task<BaseResult<SearchFlightResponse>> SearchFlightAsync(SearchFlightRequest request, CancellationToken cancellationToken = default)
    {
        // Get config
        var info = await GetConfigDataAsync(cancellationToken);

        // Request to 3th
        request.Username = info.Config!.Username;
        request.Password = info.Config.Password;

        var baseResponse = await customHttpClient.SendAsync<SearchFlightResponse>(new MyHttpRequest
        {
            Uri = new Uri(info.Api!.GetSearchFlightUri()),
            Payload = request.MySerialize(),
            MyHttpMethod = MyHttpMethod.POST,
            NumberRetry = 2,
            EnableVerifyTls = info.Api.EnableVerifyTls
        }, cancellationToken);

        // Process result
        if (!baseResponse.isSuccess)
            return GetBaseResult<SearchFlightResponse>(CodeMessage._100);

        return GetBaseResult(CodeMessage._0000, baseResponse.data);
    }
    
    public async Task<BaseResult<VerifyFlightResponse>> VerifyFlightAsync(VerifyFlightRequest request, CancellationToken cancellationToken = default)
    {
        // Get config
        var info = await GetConfigDataAsync(cancellationToken);

        // Request to 3th
        request.Username = info.Config!.Username;
        request.Password = info.Config.Password;

        var baseResponse = await customHttpClient.SendAsync<VerifyFlightResponse>(new MyHttpRequest
        {
            Uri = new Uri(info.Api!.GetVerifyFlightUri()),
            Payload = request.MySerialize(),
            MyHttpMethod = MyHttpMethod.POST,
            NumberRetry = 2,
            EnableVerifyTls = info.Api.EnableVerifyTls
        }, cancellationToken);

        // Process result
        if (!baseResponse.isSuccess)
            return GetBaseResult<VerifyFlightResponse>(CodeMessage._100);

        return GetBaseResult(CodeMessage._0000, baseResponse.data);
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
                info.Config.Password = configuration.Value;
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
        }

        _abTripInfo = info;

        return info;
    }

    #endregion
}