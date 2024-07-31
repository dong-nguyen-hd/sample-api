namespace EPAY.AIRWAY.KIOSK.API.Domain.Services;

public interface ISignalRService : IBaseService
{
    /// <summary>
    /// Chức năng: public bản tin cho subcriber
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    Task PublicMessage<T>(T obj) where T : class, new();
}
