namespace EPAY.AIRWAY.KIOSK.API.Domain.Services;

public interface ICacheService : IBaseService
{
    Task<T?> GetDataAsync<T>(string? key);

    Task SetDataAsync<T>(string key, T value, TimeSpan cacheDuration, bool isAbsoluteExpiration = true);

    Task RemoveDataAsync(string key);
}