using AIRWAY.KIOSK.API.Controllers.Config.Permission.Handler;
using AIRWAY.KIOSK.API.Controllers.Config.Permission.Requirement;
using AIRWAY.KIOSK.API.Controllers.Filters;
using AIRWAY.KIOSK.API.Controllers.Middlewares;
using AIRWAY.KIOSK.API.Domain.Services;
using AIRWAY.KIOSK.API.Resources.DTOs.Account.Mapping;
using AIRWAY.KIOSK.API.Resources.DTOs.Account.Validation;
using AIRWAY.KIOSK.API.Services;
using AIRWAY.KIOSK.API.Services.Configuration;
using AIRWAY.KIOSK.API.Services.Log;
using AIRWAY.KIOSK.API.Services.ThirdParty;
using FluentValidation;

namespace AIRWAY.KIOSK.API.Extensions.AddConfig;

public static class RelateServices
{
    public static void AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        #region Scoped

        services.AddScoped<IIntegratedIAcvService, IntegratedIAcvService>();
        
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IAbTripService, AbTripService>();
        services.AddScoped<IDeviceService, DeviceService>();

        services.AddScoped<IPaymentGatewayService, PaymentGatewayService>();
        services.AddScoped<IFlightService, FlightService>();
        services.AddScoped<ILogModelCreator, LogModelCreator>();

        services.AddScoped<IConfigurationService, ConfigurationService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ITokenManagementService, TokenManagementService>();

        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<IAuthorizationHandler, PermissionHandler>();
        services.AddAutoMapper(typeof(ResourceToModelProfile));
        services.AddValidatorsFromAssemblyContaining<CreateValidator>();

        #endregion

        #region Transient

        services.AddTransient<ISignalRService, SignalRService>();
        services.AddTransient<ILogService, LogService>();
        services.AddTransient<ICustomHttpClient, CustomHttpClient>();

        #endregion

        #region Filter

        services.AddMvc(options => { options.Filters.Add(new LoggerActionFilter()); });

        #endregion
    }

    public static void AddPolices(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(MyPolicy.Administrator, policy =>
                policy.AddRequirements(new PermissionRequirement([MyPolicy.Administrator])));

            options.AddPolicy(MyPolicy.ThirdPartyIntegration, policy =>
                policy.AddRequirements(new PermissionRequirement([MyPolicy.Administrator, MyPolicy.ThirdPartyIntegration])));

            options.AddPolicy(MyPolicy.Editor, policy =>
                policy.AddRequirements(new PermissionRequirement([MyPolicy.Administrator, MyPolicy.Editor])));

            options.AddPolicy(MyPolicy.Device, policy =>
                policy.AddRequirements(new PermissionRequirement([MyPolicy.Administrator, MyPolicy.Editor, MyPolicy.Device])));

            options.AddPolicy(MyPolicy.Viewer, policy =>
                policy.AddRequirements(new PermissionRequirement([MyPolicy.Administrator, MyPolicy.Editor, MyPolicy.Device, MyPolicy.Viewer])));
        });
    }
}