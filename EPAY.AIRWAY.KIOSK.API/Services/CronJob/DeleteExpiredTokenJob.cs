using EPAY.AIRWAY.KIOSK.API.Domain.Context;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Extensions.AddConfig;
using Microsoft.EntityFrameworkCore;

namespace EPAY.AIRWAY.KIOSK.API.Services.CronJob;

public sealed class DeleteExpiredTokenJob : CronJobService
{
    #region Properties

    private readonly IServiceProvider _serviceProvider;

    #endregion

    #region Constructor

    public DeleteExpiredTokenJob(IServiceProvider serviceProvider,
        IScheduleConfig<DeleteExpiredTokenJob> config) : base(config.CronExpression, config.TimeZoneInfo)
    {
        _serviceProvider = serviceProvider;
    }

    #endregion

    #region Method

    public override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(Random.Shared.Next(60, 600), cancellationToken);
            JobContext.LogWithContext().Information($"{nameof(DeleteExpiredTokenJob)} is working.");

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CoreContext>();
            var configurationService = scope.ServiceProvider.GetRequiredService<IConfigurationService>();

            await ProcesExpiredJobAsync(context, configurationService, cancellationToken);
        }
        catch (Exception ex)
        {
            JobContext.LogWithContext().Error($"{nameof(DeleteExpiredTokenJob)} fail: {ex.Message}", ex);
        }
    }

    #region Private work

    private async Task ProcesExpiredJobAsync(CoreContext context, IConfigurationService configurationService, CancellationToken cancellationToken)
    {
        // Lấy ra mốc hết hạn token từ config
        int intParsed = 0;
        var configurationResult = await configurationService.GetByKeyAsync(SystemConfig.SystemExpiredTokenDays, cancellationToken);
        if (configurationResult.CodeMessage == CodeMessage._99)
            intParsed = int.Parse(configurationResult.Data!.Value!);

        DateTime pivot = DateTime.UtcNow.Subtract(TimeSpan.FromDays(intParsed));

        // Delte expired webhook from DB
        await context.RefreshTokens
            .Where(x => x.ExpiredUtc >= pivot)
            .ExecuteDeleteAsync(cancellationToken);
    }

    #endregion

    #endregion
}