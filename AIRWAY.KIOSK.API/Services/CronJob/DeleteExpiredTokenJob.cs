using AIRWAY.KIOSK.API.Domain.Context;
using AIRWAY.KIOSK.API.Domain.Services;
using AIRWAY.KIOSK.API.Extensions.AddConfig;
using Microsoft.EntityFrameworkCore;

namespace AIRWAY.KIOSK.API.Services.CronJob;

/// <summary>
/// Job xoá thông tin token hết hạn
/// </summary>
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

    protected override async Task DoWorkAsync(CancellationToken cancellationToken)
    {
        string jobId = string.Empty;

        try
        {
            await Task.Delay(Random.Shared.Next(1000, 9999), cancellationToken);
            jobId = RelateText.GenId();
            JobContext.LogWithContext().Information($"{nameof(DeleteExpiredTokenJob)} ({jobId}) is working.");

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CoreContext>();

            // Kiểm tra đã có job trước đó thực hiện tác vụ này chưa.
            var value = await context.CronJobFlags.FirstOrDefaultAsync(x => x.Name == $"{nameof(DeleteExpiredTokenJob)}", cancellationToken);
            if (value != null)
            {
                // Không thực hiện lại tác vụ nếu đã thực hiện trước đó trong khoảng 5 phút
                var utcNow = DateTime.UtcNow;
                if (utcNow.Subtract(value.UpdatedDatetimeUtc).TotalSeconds <= (5 * 60))
                {
                    JobContext.LogWithContext().Information($"{nameof(DeleteExpiredTokenJob)} ({jobId}) is cancel");
                    return;
                }
                
                value.UpdatedDatetimeUtc = utcNow;
                context.CronJobFlags.Update(value);
                await context.SaveChangesAsync(cancellationToken);
                
                var configurationService = scope.ServiceProvider.GetRequiredService<IConfigurationService>();
                await ProcesExpiredJobAsync(context, configurationService, cancellationToken);
            }
            else
            {
                JobContext.LogWithContext().Information($"{nameof(DeleteExpiredTokenJob)} ({jobId}) is cancel");
            }
        }
        catch (Exception ex)
        {
            if (ex is DbUpdateConcurrencyException)
                JobContext.LogWithContext().Information($"{nameof(DeleteExpiredTokenJob)} ({jobId}) is cancel");
            else
                JobContext.LogWithContext().Error($"{nameof(DeleteExpiredTokenJob)} ({jobId}) is fail: {ex.Message}", ex);
        }
    }

    #region Private work

    private async Task ProcesExpiredJobAsync(CoreContext context, IConfigurationService configurationService, CancellationToken cancellationToken)
    {
        // Lấy ra mốc hết hạn token từ config
        var configurationResult = await configurationService.GetByKeyAsync(SystemConfig.SystemExpiredTokenDays, cancellationToken);

        if (configurationResult.CodeMessage == CodeMessage._0000 &&
            int.TryParse(configurationResult.Data!.Value!, out int intParsed) &&
            intParsed > 0)
        {
            DateTime pivot = DateTime.UtcNow.Subtract(TimeSpan.FromDays(intParsed));

            // Delte expired webhook from DB
            await context.RefreshTokens
                .Where(x => x.ExpiredUtc <= pivot)
                .ExecuteDeleteAsync(cancellationToken);
        }
    }

    #endregion

    #endregion
}