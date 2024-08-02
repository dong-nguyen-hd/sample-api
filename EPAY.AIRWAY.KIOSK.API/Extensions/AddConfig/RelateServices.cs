using EPAY.AIRWAY.KIOSK.API.Controllers.Config.Permission.Handler;
using EPAY.AIRWAY.KIOSK.API.Controllers.Config.Permission.Requirement;
using EPAY.AIRWAY.KIOSK.API.Controllers.Middlewares;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Account.Mapping;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Account.Validation;
using EPAY.AIRWAY.KIOSK.API.Services;
using EPAY.AIRWAY.KIOSK.API.Services.Log;
using EPAY.AIRWAY.KIOSK.API.Services.ThirdParty;
using FluentValidation;

namespace EPAY.AIRWAY.KIOSK.API.Extensions.AddConfig;

public static class RelateServices
{
    public static void AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICustomHttpClient, CustomHttpClient>();
        services.AddScoped<IAbTripService, AbTripService>();
        services.AddScoped<IPaymentGatewayService, PaymentGatewayService>();
        services.AddScoped<IFlightService, FlightService>();

        services.AddScoped<ILogModelCreator, LogModelCreator>();
        services.AddScoped<ILogService, LogService>();
        services.AddScoped<IConfigurationService, ConfigurationService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ITokenManagementService, TokenManagementService>();

        services.AddScoped<IAuthorizationHandler, PermissionHandler>();
        services.AddTransient<ISignalRService, SignalRService>();
        services.AddAutoMapper(typeof(ResourceToModelProfile));
        services.AddValidatorsFromAssemblyContaining<CreateValidator>();
    }

    public static void AddPolices(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(MyPolicy.Administrator, policy =>
                policy.AddRequirements(new PermissionRequirement([MyPolicy.Administrator])));

            options.AddPolicy(MyPolicy.Editor, policy =>
                policy.AddRequirements(new PermissionRequirement([MyPolicy.Administrator, MyPolicy.Editor])));

            options.AddPolicy(MyPolicy.Device, policy =>
                policy.AddRequirements(new PermissionRequirement([MyPolicy.Administrator, MyPolicy.Editor, MyPolicy.Device])));

            options.AddPolicy(MyPolicy.Viewer, policy =>
                policy.AddRequirements(new PermissionRequirement([MyPolicy.Administrator, MyPolicy.Editor, MyPolicy.Device, MyPolicy.Viewer])));
        });
    }
}