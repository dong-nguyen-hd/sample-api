using Dapper;
using EPAY.AIRWAY.KIOSK.API.Domain.Context;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace EPAY.AIRWAY.KIOSK.API.Services.Log;

public partial class LogService(CoreContext context) : BaseService, ILogService
{
    #region Method

    public async Task<Model.Log?> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        await context.Logs.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<bool> CreateAsync(Model.Log log, CancellationToken cancellationToken = default)
    {
        try
        {
            // Excute
            var query = CreateQuery(log);

            await using var conn = new NpgsqlConnection(SystemGlobal.PostgresqlConnectionString);
            var res = await conn.ExecuteAsync(query.sql, query.param, commandTimeout: SystemConstant.TimeOutDefault);

            // Process result
            return res > 0;
        }
        catch (Exception e)
        {
            Serilog.Log.Error(e, $"{e.Message}: {log.MySerialize()}");
            throw;
        }
    }

    public async Task<bool> DeleteExpiredAsync(DateTime pivot, CancellationToken cancellationToken = default)
    {
        try
        {
            // Excute
            var query = DeleteExpiredQuery(pivot);

            await using var conn = new NpgsqlConnection(SystemGlobal.PostgresqlConnectionString);
            var res = await conn.ExecuteAsync(query.sql, param: query.param, commandTimeout: SystemConstant.TimeOutDefault);

            // Process result
            return res > 0;
        }
        catch (Exception e)
        {
            Serilog.Log.Error(e, e.Message);
            throw;
        }
    }

    #endregion
}