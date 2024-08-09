using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace EPAY.AIRWAY.KIOSK.API.Services;

public sealed class CacheService(IDistributedCache cache) : BaseService, ICacheService
{
    #region Method

    public async Task<T?> GetDataAsync<T>(string? key)
    {
        if (string.IsNullOrEmpty(key.RemoveAllSpaceChar()))
            return default;

        var value = await cache.GetStringAsync(key!);

        if (string.IsNullOrEmpty(value))
            return default;

        return JsonSerializer.Deserialize<T>(value);
    }

    public async Task SetDataAsync<T>(string key, T value, TimeSpan cacheDuration, bool isAbsoluteExpiration = true)
    {
        if (string.IsNullOrEmpty(key.RemoveAllSpaceChar()) || value is null)
            return;

        var options = isAbsoluteExpiration ? new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = cacheDuration } : new DistributedCacheEntryOptions { SlidingExpiration = cacheDuration };

        var jsonData = value.MySerialize();
        
        await cache.SetStringAsync(key.RemoveAllSpaceChar(), jsonData, options);
    }

    public async Task RemoveDataAsync(string key)
    {
        await cache.RemoveAsync(key);
    }

    #endregion
}