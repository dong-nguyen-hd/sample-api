using EPAY.AIRWAY.KIOSK.API.Domain.Context;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Account.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Account.Response;
using Microsoft.EntityFrameworkCore;

namespace EPAY.AIRWAY.KIOSK.API.Services;

public sealed class AccountService(IMapper mapper, CoreContext context) : BaseService, IAccountService
{
    #region Method

    public async Task<BaseResult<AccountResponse>> CreateAsync(CreateRequest request, CancellationToken cancellationToken = default)
    {
        // Xác thực user-name hợp lệ
        var accountDb = await context.Accounts
            .SingleOrDefaultAsync(x => x.UserName == request.UserName.ToLowerAndRemoveSpace(), cancellationToken);
        if (accountDb != null)
            return GetBaseResult<AccountResponse>(CodeMessage._100);

        // Mapping Resource to Account
        var tempAccount = mapper.Map<Model.Account>(request);

        await context.Accounts.AddAsync(tempAccount, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // Process result
        var result = mapper.Map<AccountResponse>(tempAccount);
        return GetBaseResult(CodeMessage._99, data: result);
    }

    public async Task<BaseResult<AccountResponse>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var account = await context.Accounts
            .Select(x => new Model.Account()
            {
                Id = x.Id
            })
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (account == null)
            return GetBaseResult<AccountResponse>(CodeMessage._100);

        context.Accounts.Remove(account);
        await context.SaveChangesAsync(cancellationToken);

        return GetBaseResult(CodeMessage._99, data: mapper.Map<AccountResponse>(account));
    }

    public async Task<BaseResult<AccountResponse>> UpdatePasswordAsync(int id, UpdatePasswordAccountRequest request, CancellationToken cancellationToken = default)
    {
        // Xác thực Id có tồn tại?
        var accountDb = await context.Accounts
            .Select(x => new Model.Account()
            {
                Id = x.Id,
                Password = x.Password
            })
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (accountDb == null)
            return GetBaseResult<AccountResponse>(CodeMessage._100);

        if (accountDb.Password.CheckingPassword(request.OldPassword))
            return GetBaseResult<AccountResponse>(CodeMessage._100);

        // Cập nhật password
        accountDb.Password = request.NewPassword.HashingPassword();
        accountDb.UpdatedDatetimeUtc = DateTime.UtcNow;
        context.Accounts.Update(accountDb);
        await context.SaveChangesAsync(cancellationToken);

        // Xoá tất cả token khi thay đổi mật khẩu
        await context.RefreshTokens
            .Where(x => x.AccountId == id)
            .ExecuteUpdateAsync(x => x.SetProperty(y => y.IsUsed, false), cancellationToken);

        return GetBaseResult(CodeMessage._99, data: mapper.Map<AccountResponse>(accountDb));
    }

    public async Task<BaseResult<AccountResponse>> UpdateAsync(int id, UpdateRequest request, CancellationToken cancellationToken = default)
    {
        // Xác thực Id có tồn tại?
        var accountDb = await context.Accounts.SingleOrDefaultAsync(x => x.Id == id);
        if (accountDb == null)
            return GetBaseResult<AccountResponse>(CodeMessage._100);

        // Cập nhật account
        mapper.Map(request, accountDb);

        // Gán role vào account
        var dataResult = mapper.Map<AccountResponse>(accountDb);
        context.Accounts.Update(accountDb);
        await context.SaveChangesAsync(cancellationToken);

        return GetBaseResult(CodeMessage._99, data: dataResult);
    }

    #endregion
}