using EPAY.AIRWAY.KIOSK.API.Domain.Context;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Configuration.Response;
using Microsoft.EntityFrameworkCore;

namespace EPAY.AIRWAY.KIOSK.API.Services;

public sealed class ConfigurationService(IMapper mapper, CoreContext context) : BaseService(mapper, context), IConfigurationService
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
            configuration = await Mapper.ProjectTo<ConfigurationResponse>(Context.Configurations)
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Key == key, cancellationToken);
        }

        if (configuration == null)
            return GetBaseResult<ConfigurationResponse>(CodeMessage._100);

        return GetBaseResult(CodeMessage._99, data: configuration);
    }

    public async Task<BaseResult<List<ConfigurationResponse>>> GetByKeysAsync(string[] keys, CancellationToken cancellationToken = default)
    {
        List<ConfigurationResponse>? configurations = new();

        if (_configurationResponses != null && _configurationResponses.Count > 0)
        {
            configurations = _configurationResponses.Where(x => keys.Contains(x.Key)).ToList();
        }
        else
        {
            configurations = await Mapper.ProjectTo<ConfigurationResponse>(Context.Configurations
                    .AsNoTracking()
                    .Where(x => keys.Contains(x.Key)))
                .ToListAsync(cancellationToken);
        }

        if (configurations.Count <= 0)
            return GetBaseResult<List<ConfigurationResponse>>(CodeMessage._100);

        return GetBaseResult(CodeMessage._99, data: configurations);
    }

    public async Task<BaseResult<List<ConfigurationResponse>>> GetAllAsync(bool excludeInternal, CancellationToken cancellationToken = default)
    {
        // Lấy dữ liệu từ cache trong trường hợp có dữ liệu
        if (_configurationResponses != null && _configurationResponses.Count > 0)
            return GetBaseResult(CodeMessage._99, data: _configurationResponses.ToList());

        // Tiếp tục lấy dữ liệu từ DB trong trường hợp không có dữ liệu
        var queryable = Context.Configurations.AsQueryable().AsNoTracking();

        if (excludeInternal)
            queryable = queryable.Where(x => !x.Internal);

        var configurations = await Mapper.ProjectTo<ConfigurationResponse>(queryable).ToListAsync(cancellationToken);

        if (configurations.Count <= 0)
            return GetBaseResult<List<ConfigurationResponse>>(CodeMessage._100);

        // Cập nhật dữ liệu trong cache
        _configurationResponses = configurations.ToHashSet();

        return GetBaseResult(CodeMessage._99, data: configurations);
    }

    #endregion
}