using Dapper;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Configuration.Response;
using Npgsql;

namespace EPAY.AIRWAY.KIOSK.API.Services.Configuration;

public sealed partial class ConfigurationService : BaseService, IConfigurationService
{
    #region Properties

    /// <summary>
    /// Phục vụ cache level 1
    /// </summary>
    private HashSet<ConfigurationResponse>? _configurationResponses = new();

    #endregion

    #region Method

    public async Task<BaseResult<ConfigurationResponse>> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        ConfigurationResponse? configuration = new();

        if (_configurationResponses != null && _configurationResponses.Count > 0)
        {
            configuration = _configurationResponses.FirstOrDefault(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase));
        }
        else
        {
            var query = GetByKeyQuery(key);
            await using var conn = new NpgsqlConnection(SystemGlobal.PostgresqlConnectionString);
            configuration = await conn.QuerySingleOrDefaultAsync<ConfigurationResponse>(query.sql, query.param, commandTimeout: SystemConstant.TimeOutDefault);
        }

        if (configuration == null)
            return GetBaseResult<ConfigurationResponse>(CodeMessage._100);

        return GetBaseResult(CodeMessage._0000, data: configuration);
    }

    public async Task<BaseResult<List<ConfigurationResponse>>> GetByKeysAsync(string[] keys, CancellationToken cancellationToken = default)
    {
        List<ConfigurationResponse>? configurations = [];

        if (_configurationResponses != null && _configurationResponses.Count > 0)
        {
            configurations = _configurationResponses.Where(x => keys.Contains(x.Key)).ToList();
        }
        else
        {
            var query = GetByKeysQuery(keys);
            await using var conn = new NpgsqlConnection(SystemGlobal.PostgresqlConnectionString);
            configurations = (await conn.QueryAsync<ConfigurationResponse>(query.sql, query.param, commandTimeout: SystemConstant.TimeOutDefault)).ToList();
        }

        if (configurations.Count <= 0)
            return GetBaseResult<List<ConfigurationResponse>>(CodeMessage._100);

        return GetBaseResult(CodeMessage._0000, data: configurations);
    }

    public async Task<BaseResult<List<ConfigurationResponse>>> GetAllAsync(bool excludeInternal, CancellationToken cancellationToken = default)
    {
        // Lấy dữ liệu từ cache trong trường hợp có dữ liệu
        if (_configurationResponses is { Count: > 0 })
            return GetBaseResult(CodeMessage._0000, data: _configurationResponses.ToList());

        // Tiếp tục lấy dữ liệu từ DB trong trường hợp không có dữ liệu
        var query = GetAllQuery(excludeInternal);
        await using var conn = new NpgsqlConnection(SystemGlobal.PostgresqlConnectionString);
        var configurations = (await conn.QueryAsync<ConfigurationResponse>(query.sql, query.param, commandTimeout: SystemConstant.TimeOutDefault)).ToList();

        if (configurations.Count <= 0)
            return GetBaseResult<List<ConfigurationResponse>>(CodeMessage._100);

        // Cập nhật dữ liệu trong cache
        _configurationResponses = configurations.ToHashSet();

        return GetBaseResult(CodeMessage._0000, data: configurations);
    }

    #endregion
}