using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Configuration.Response;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Services;

public interface IConfigurationService : IBaseService
{
    /// <summary>
    /// Chức năng: lấy giá trị config bằng key
    /// </summary>
    /// <param name="key"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<ConfigurationResponse>> GetByKeyAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: lấy giá trị config bằng nhiều key
    /// </summary>
    /// <param name="keys"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<List<ConfigurationResponse>>> GetByKeysAsync(string[] keys, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: lấy ra tất cả giá trị config, có thể lọc theo internal (chỉ được phép dùng trong hệ thống)
    /// </summary>
    /// <param name="excludeInternal"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<List<ConfigurationResponse>>> GetAllAsync(bool excludeInternal, CancellationToken cancellationToken = default);
}