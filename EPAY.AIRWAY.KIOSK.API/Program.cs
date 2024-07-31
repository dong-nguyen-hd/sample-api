using System.Text;
using EPAY.AIRWAY.KIOSK.API.Controllers.Config;
using EPAY.AIRWAY.KIOSK.API.Controllers.Middlewares;
using EPAY.AIRWAY.KIOSK.API.Domain.Context;
using EPAY.AIRWAY.KIOSK.API.Extensions.AddConfig;
using EPAY.AIRWAY.KIOSK.API.Extensions.JsonConverter;
using Hangfire;
using Hangfire.InMemory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

const string _hostingSourceContext = "Microsoft.Hosting.Lifetime";

try
{
    Console.OutputEncoding = Encoding.UTF8;

    var builder = WebApplication.CreateBuilder(args);

    // Declare external-file
    builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    builder.Configuration.AddJsonFile("response-message.json", optional: false, reloadOnChange: true);
    builder.Configuration.AddUserSecrets<Program>(false); // Explicit use secrets.json in env production, staging. By default it only use in development

    SystemGlobal.IsDebug = builder.Environment.IsDevelopment();
    builder.Services.GetSystemData(builder.Configuration);

    // Declare log
    builder.Services.AddLog(builder.Configuration);
    _hostingSourceContext.LogWithContext().Information("Starting up");
    builder.Host.UseSerilog();

    // Force convert dateTime with kind in PostgreSQL
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

    #region Add services to the container.

    builder.Services.AddSignalR();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddControllers(opt =>
    {
        opt.ApplyProfile(); // Add custom cache profile
    }).ConfigureApiBehaviorOptions(options =>
    {
        // Adds a custom error response factory when Model-State is invalid
        options.InvalidModelStateResponseFactory = InvalidResponseFactory.ProduceErrorResponse;
    }).AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new MyDateTimeConverter());
        options.JsonSerializerOptions.Converters.Add(new MyDecimalConverter());
    });

    // Add redis / mem cache
    if (CacheConfig.UseRedis)
    {
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = CacheConfig.RedisUri;
            options.InstanceName = CacheConfig.RedisInstanceName;
        });
    }
    else
    {
        builder.Services.AddDistributedMemoryCache();
    }

    // Add hangfire
    builder.Services.AddHangfire(options =>
    {
        options
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseInMemoryStorage(
                new InMemoryStorageOptions
                {
                    MaxExpirationTime = TimeSpan.FromMinutes(20)
                });
    });
    builder.Services.AddHangfireServer();
    builder.Services.RegisterCronJob();

    builder.Services.AddResponseCaching();
    builder.Services.AddJwtBearerAuthentication();
    builder.Services.AddCustomizeSwagger();
    builder.Services.AddEndpointsApiExplorer(); // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddSwaggerGen();
    builder.Services.AddDbContext<CoreContext>(opts =>
    {
        opts.UseNpgsql(SystemGlobal.PostgresqlConnectionString, o =>
        {
            o.EnableRetryOnFailure();

            if (SystemGlobal.IsDebug)
            {
                opts.EnableDetailedErrors();
                opts.EnableSensitiveDataLogging();
            }
        }).UseSnakeCaseNamingConvention();
    });

    builder.Services.AddPolices(); // Policy-based authorization
    builder.Services.AddDependencyInjection(builder.Configuration);
    builder.Services.RegisterHttpClient(builder.Configuration);
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });
    });

    #endregion

    #region Configure the HTTP request pipeline.

    var app = builder.Build();
    app.UseStaticFiles();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options => { options.DefaultModelsExpandDepth(-1); });
        app.UseHangfireDashboard();
    }

    app.UseSerilogRequestLogging();
    if (app.Environment.IsProduction())
        app.UseHttpsRedirection();
    app.UseCors("AllowAll");
    app.UseRouting();
    app.UseResponseCaching();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseMiddleware<LoggerMiddleware>();
    app.UseMiddleware<ErrorHandlerMiddleware>();
    app.Use((context, next) => // No-caching explicit
    {
        context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
        {
            NoCache = true,
            NoStore = true
        };
        return next.Invoke();
    });
    app.MapControllers();
    app.UseHub();
    app.Run();

    #endregion
}
catch (Exception ex)
{
    if (ex is HostAbortedException) // Ex throw by ef-core when migration
        return;

    _hostingSourceContext.LogWithContext().Fatal($"Unhandled exception: {ex.Message}", ex);
}
finally
{
    _hostingSourceContext.LogWithContext().Information("Shut down complete");
    Log.CloseAndFlush();
}