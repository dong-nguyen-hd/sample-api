namespace AIRWAY.KIOSK.API.Domain.Services;

public interface ICacheService : IBaseService
{
    Task<T?> GetDataAsync<T>(string? key, CancellationToken cancellationToken = default);

    Task SetDataAsync<T>(string key, T value, TimeSpan cacheDuration, bool isAbsoluteExpiration = true, CancellationToken cancellationToken = default);

    Task RemoveDataAsync(string key, CancellationToken cancellationToken = default);
}