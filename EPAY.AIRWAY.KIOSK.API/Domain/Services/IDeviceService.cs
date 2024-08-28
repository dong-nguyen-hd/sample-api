using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Device.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Device.Response;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Services;

public interface IDeviceService : IBaseService
{
    /// <summary>
    /// Chức năng: lấy thông tin device bằng mã mã thiết bị
    /// </summary>
    /// <param name="code"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<DeviceResponse>> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: tạo mới thông tin thiết bị
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<DeviceResponse>> CreateAsync(CreateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: cập nhật thông tin thiết bị
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<DeviceResponse>> UpdateAsync(string id, UpdateRequest request, CancellationToken cancellationToken = default);
}