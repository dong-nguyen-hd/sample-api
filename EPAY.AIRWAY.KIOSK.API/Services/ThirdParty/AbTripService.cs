using EPAY.AIRWAY.KIOSK.API.Domain.Context;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.CustomHttpClient.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip.Response;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.PaymentGateway.Response;
using EPAY.AIRWAY.KIOSK.API.Resources.Enums;
using EPAY.AIRWAY.KIOSK.API.Resources.Exceptions;
using EPAY.AIRWAY.KIOSK.API.Resources.SystemData.ThirdParty.AbTrip;

namespace EPAY.AIRWAY.KIOSK.API.Services.ThirdParty;

public class AbTripService(
    IConfigurationService configurationService,
    ICustomHttpClient customHttpClient,
    IMapper mapper,
    CoreContext context) : BaseService(mapper, context), IAbTripService
{
    #region Method

    public async Task<BaseResult<AircraftsResponse>> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        // Get config
        var info = await GetConfigDataAsync(cancellationToken);

        // Request to 3th
        AircraftsRequest payload = new()
        {
            Username = info.Config.Username,
            Password = info.Config.Password
        };
        var baseResponse = await customHttpClient.SendAsync<AircraftsResponse>(new MyHttpRequest
        {
            Uri = new Uri(info.Api.GetAircraftsUri()),
            Payload = payload.MySerialize(),
            MyHttpMethod = MyHttpMethod.GET,
            NumberRetry = 2,
            EnableVerifyTls = info.Api.EnableVerifyTls
        }, cancellationToken);

        // Process result
        if (!baseResponse.isSuccess)
            return GetBaseResult<AircraftsResponse>(CodeMessage._100);

        return GetBaseResult(CodeMessage._99, baseResponse.data);
    }

    public async Task<AbTripInfo> GetConfigDataAsync(CancellationToken cancellationToken = default)
    {
        // Get config from DB
        var configurations = await configurationService.GetAllAsync(false, cancellationToken);

        if (configurations.CodeMessage != CodeMessage._99)
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

        return info;
    }

    #region Private work

    #endregion

    #endregion
}