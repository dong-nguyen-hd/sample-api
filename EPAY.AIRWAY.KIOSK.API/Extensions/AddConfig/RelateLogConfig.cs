using Serilog.Sinks.Elasticsearch;

namespace EPAY.AIRWAY.KIOSK.API.Extensions.AddConfig;

using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Filters;
using Serilog.Formatting.Json;

public static class RelateLogConfig
{
    public static ILogger LogWithContext(this string context) =>
        Log.ForContext("SourceContext", context);

    public static void AddLog(this IServiceCollection services, IConfiguration configuration)
    {
        #region Config log-type

        var logCfg = new LoggerConfiguration();

        logCfg
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentUserName()
            .Enrich.WithProperty("ApplicationName", SystemInformation.ApplicationName);
        
        if (SystemGlobal.IsDebug) // Enable log query EF Core for dev env
            logCfg.MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information);

        if (SerilogConfig.EnableConsoleLog)
            logCfg.WriteTo.Console();
        else
        {
            // Log only important information
            logCfg.WriteTo.Logger(lc =>
                lc.Filter.ByIncludingOnly(Matching.WithProperty<string>("SourceContext", p => p == "Microsoft.Hosting.Lifetime"))
                    .WriteTo.Console());
        }

        if (SerilogConfig.EnableFileLog)
            logCfg.WriteTo.File(new JsonFormatter(), SerilogConfig.PathFileLog ?? "./logs/.json", rollingInterval: RollingInterval.Day);

        if (SerilogConfig.EnableElasticsearchLog)
        {
            logCfg.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(SerilogConfig.ElasticsearchUri))
            {
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8,
                ModifyConnectionSettings = (settings) =>
                {
                    settings.ServerCertificateValidationCallback((o, certificate, arg3, arg4) => true);
                    settings.BasicAuthentication(SerilogConfig.ElasticsearchUsername, SerilogConfig.ElasticsearchUsername);
                    return settings;
                },
                IndexFormat = SerilogConfig.ElasticsearchIndexFormat
            });
        }

        Log.Logger = logCfg.CreateLogger();

        #endregion
    }
}
