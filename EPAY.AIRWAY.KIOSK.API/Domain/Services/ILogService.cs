namespace EPAY.AIRWAY.KIOSK.API.Domain.Services;

public interface ILogService : IBaseService
{
    /// <summary>
    /// Chức năng: tạo bản ghi log vào DB
    /// </summary>
    /// <param name="log"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> CreateAsync(Model.Log log, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: xoá bản ghi log theo mốc pivot. <br/>
    /// Điều kiện xoá: request_datetime_utc <= pivot <br/>
    /// </summary>
    /// <param name="pivot"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> DeleteExpiredAsync(DateTime pivot, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: lấy ra thông tin Log dựa vào Id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Model.Log?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
}