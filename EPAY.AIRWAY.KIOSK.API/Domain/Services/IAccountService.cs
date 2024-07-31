using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Account.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Account.Response;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Services;

public interface IAccountService : IBaseService
{
    /// <summary>
    /// Chức năng: tạo mới một tài khoản
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<AccountResponse>> CreateAsync(CreateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: xoá một tài khoản
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<AccountResponse>> DeleteAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: cập nhật mật khẩu
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<AccountResponse>> UpdatePasswordAsync(int id, UpdatePasswordAccountRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Chức năng: cập nhật thông tin tài khoản
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<BaseResult<AccountResponse>> UpdateAsync(int id, UpdateRequest request, CancellationToken cancellationToken = default);
}