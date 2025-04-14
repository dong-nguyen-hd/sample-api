using AIRWAY.KIOSK.API.Services.CronJob;
using TimeZoneConverter;

namespace AIRWAY.KIOSK.API.Extensions.AddConfig;

public static class RelateCronJob
{
    public static void RegisterCronJob(this IServiceCollection services)
    {
        // Every minute: * * * * *
        
        services.AddCronJob<DeleteExpiredTokenJob>(c =>
        {
            c.TimeZoneInfo = TZConvert.GetTimeZoneInfo(SystemConstant.VietnamTimeZoneId);
            c.CronExpression = @"0 3 * * *"; // Every 3h AM
        });
        
        services.AddCronJob<DeleteExpiredLogJob>(c =>
        {
            c.TimeZoneInfo = TZConvert.GetTimeZoneInfo(SystemConstant.VietnamTimeZoneId);
            c.CronExpression = @"5 3 * * *"; // Every 3h5m AM
        });
        
        services.AddCronJob<PaymentReportJob>(c =>
        {
            c.TimeZoneInfo = TZConvert.GetTimeZoneInfo(SystemConstant.VietnamTimeZoneId);
            c.CronExpression = @"0 6 * * *"; // Every 6h AM
        });
    }

    #region Private work
    
    private static void AddCronJob<T>(this IServiceCollection services, Action<IScheduleConfig<T>> options) where T : CronJobService
    {
        if (options == null)
            throw new ArgumentNullException(nameof(options), "Please provide Schedule Configurations.");

        var config = new ScheduleConfig<T>();
        options.Invoke(config);

        if (string.IsNullOrWhiteSpace(config.CronExpression))
            throw new ArgumentNullException(nameof(ScheduleConfig<T>.CronExpression), "Empty Cron Expression is not allowed.");

        services.AddSingleton<IScheduleConfig<T>>(config);
        services.AddSingleton<T>();
        services.AddHostedService<T>();
    }

    #endregion
}

public interface IScheduleConfig<T>
{
    #region Properties

    string? CronExpression { get; set; }
    TimeZoneInfo? TimeZoneInfo { get; set; }

    #endregion
}

public sealed class ScheduleConfig<T> : IScheduleConfig<T>
{
    #region Properties

    public string? CronExpression { get; set; }
    public TimeZoneInfo? TimeZoneInfo { get; set; }

    #endregion
}