namespace AIRWAY.KIOSK.API.Domain.Services;

public interface ISignalRService : IBaseService
{
    /// <summary>
    /// Chức năng: public bản tin cho subcriber
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task PublicMessageAsync<T>(T? obj, CancellationToken cancellationToken = default) where T : class, new();
}
