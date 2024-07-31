using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using EPAY.AIRWAY.KIOSK.API.Domain.Context;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Authentication.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Authentication.Response;
using IdGen;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EPAY.AIRWAY.KIOSK.API.Services;

public sealed class TokenManagementService(IMapper mapper, CoreContext context) : BaseService(mapper, context), ITokenManagementService
{
    public async Task<BaseResult<TokenResponse>> GenerateNewTokensAsync(RefreshTokenRequest refreshTokenRequest, DateTime utcNow, CancellationToken cancellationToken = default)
    {
        // Trích xuất thông tin về refreshTokenId từ chuỗi refreshToken
        var oldRefreshTokenId = ComputeRefreshTokenId(refreshTokenRequest.RefreshToken);
        if (string.IsNullOrEmpty(oldRefreshTokenId))
            return GetBaseResult<TokenResponse>(CodeMessage._100);

        // Xác thực refreshToken
        var refreshTokenDb = await Context.RefreshTokens
            .Include(x => x.Account)
            .SingleOrDefaultAsync(x => x.Id == oldRefreshTokenId, cancellationToken);

        if (refreshTokenDb == null ||
            refreshTokenDb.Token != refreshTokenRequest.RefreshToken ||
            refreshTokenDb.IsUsed ||
            !(DateTime.Compare(refreshTokenDb.ExpiredUtc, utcNow) > 0))
            return GetBaseResult<TokenResponse>(CodeMessage._100);

        var accountDb = refreshTokenDb.Account;

        // Tạo access-token
        var accessToken = GenerateAccessToken(accountDb, utcNow);

        // Tạo refresh-token
        var newRefreshToken = GenerateRefreshToken(utcNow, refreshTokenRequest.UserAgent);
        newRefreshToken.AccountId = accountDb.Id;

        // Gán giá trị cho hoạt động cuối
        accountDb.UpdatedDatetimeUtc = DateTime.UtcNow;
        Context.Update(accountDb);

        // Vô hiệu refresh-token cũ
        refreshTokenDb.IsUsed = true;
        Context.RefreshTokens.Update(refreshTokenDb);

        // Thêm mới một refresh-token
        await Context.AddAsync(newRefreshToken, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);

        var result = new TokenResponse
        {
            Id = newRefreshToken.Id,
            AccessToken = accessToken.value,
            AccessTokenExpireTimeUTC = accessToken.expiredTime,
            RefreshTokenExpireTimeUTC = newRefreshToken.ExpiredUtc,
            RefreshToken = newRefreshToken.Token,
        };

        return GetBaseResult(CodeMessage._99, data: result);
    }

    public async Task<BaseResult<bool>> LogoutAsync(LogoutRequest logoutRequest, CancellationToken cancellationToken = default)
    {
        // Trích xuất thông tin về refreshTokenId từ chuỗi refreshToken
        var oldRefreshTokenId = ComputeRefreshTokenId(logoutRequest.RefreshToken);
        if (string.IsNullOrEmpty(oldRefreshTokenId))
            return GetBaseResult<bool>(CodeMessage._100);

        // Xác thực refresh-token
        var refreshTokenDb = await Context.RefreshTokens.SingleOrDefaultAsync(x => x.Id == oldRefreshTokenId, cancellationToken);
        if (refreshTokenDb == null || refreshTokenDb.Token != logoutRequest.RefreshToken)
            return GetBaseResult<bool>(CodeMessage._100);

        refreshTokenDb.IsUsed = true;
        Context.RefreshTokens.Update(refreshTokenDb);
        await Context.SaveChangesAsync(cancellationToken);

        return GetBaseResult<bool>(CodeMessage._99);
    }

    public async Task<BaseResult<AccessTokenResponse>> GenerateTokensAsync(LoginRequest loginRequest, DateTime utcNow, string userAgent, CancellationToken cancellationToken = default)
    {
        // Xác thực login-request
        var tempAccount = await Context.Accounts
            .AsNoTracking()
            .Select(x => new Model.Account()
            {
                Id = x.Id,
                UserName = x.UserName,
                Password = x.Password
            })
            .SingleOrDefaultAsync(x => x.UserName == loginRequest.UserName, cancellationToken);

        if (tempAccount == null)
            return GetBaseResult<AccessTokenResponse>(CodeMessage._100);
        bool isValid = tempAccount.Password.CheckingPassword(loginRequest.Password);
        if (!isValid)
            return GetBaseResult<AccessTokenResponse>(CodeMessage._100);

        // Lấy dữ liệu account sau khi đã xác thực hợp lệ
        var accountDb = await Context.Accounts.SingleOrDefaultAsync(x => x.Id == tempAccount.Id, cancellationToken);

        // Lọc theme-type
        if (accountDb == null)
            return GetBaseResult<AccessTokenResponse>(CodeMessage._100);
        if (loginRequest.Type != null && accountDb?.AdditionData?.Themes?.Count > 0)
            accountDb.AdditionData?.Themes.RemoveWhere(x => x.Type != loginRequest.Type);

        // Tạo access-token
        var accessToken = GenerateAccessToken(accountDb, utcNow);

        // Tạo refresh-token
        var refreshToken = GenerateRefreshToken(utcNow, userAgent);
        refreshToken.AccountId = accountDb.Id;

        // Gán giá trị cho hoạt động cuối
        accountDb.UpdatedDatetimeUtc = DateTime.UtcNow;
        Context.Update(accountDb);

        // Thêm mới một refresh-token
        await Context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);

        var dataResult = MappingTokenResoure(accountDb, refreshToken, accessToken.value, accessToken.expiredTime);

        return GetBaseResult(CodeMessage._99, data: dataResult);
    }

    #region Private work

    private AccessTokenResponse MappingTokenResoure(Model.Account account, Model.RefreshToken refreshToken, string accessToken, DateTime expiredTime)
    {
        var tokenResponse = Mapper.Map<AccessTokenResponse>(account);
        tokenResponse.TokenResponse = Mapper.Map<TokenResponse>(refreshToken);
        tokenResponse.TokenResponse.AccessToken = accessToken;
        tokenResponse.TokenResponse.AccessTokenExpireTimeUTC = expiredTime;

        return tokenResponse;
    }

    private (string value, DateTime expiredTime) GenerateAccessToken(Model.Account account, DateTime utcNow)
    {
        byte[] secret = Encoding.ASCII.GetBytes(JwtConfig.Secret);

        // Get claim value
        Claim[] claims = GetClaim(account, account?.SystemRoles);

        var shouldAddAudienceClaim = string.IsNullOrWhiteSpace(claims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Aud)?.Value);
        DateTime expiredAccessToken = utcNow.AddMinutes(JwtConfig.AccessTokenExpiration);

        var jwtToken = new JwtSecurityToken(
            JwtConfig.Issuer,
            shouldAddAudienceClaim ? JwtConfig.Audience : string.Empty,
            claims,
            expires: expiredAccessToken,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256Signature));

        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);

        return (accessToken, expiredAccessToken);
    }

    private static Claim[] GetClaim(Model.Account account, List<string>? nameRoles)
    {
        var claims = new List<Claim>
        {
            // Optional: you can add other Claims.
            // Note: You avoid sensitive information, because this is public.
            new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
            new Claim(ClaimTypes.Name, account.UserName),
        };

        if (nameRoles != null)
            foreach (var role in nameRoles)
                claims.Add(new Claim(ClaimTypes.Role, role));

        return claims.ToArray();
    }

    private Model.RefreshToken GenerateRefreshToken(DateTime utcNow, string? userAgent)
    {
        var refreshTokenId = new IdGenerator(0).CreateId().ToString();

        return new()
        {
            Id = refreshTokenId,
            Token = GenerateRefreshTokenString(refreshTokenId),
            ExpiredUtc = utcNow.AddMinutes(JwtConfig.RefreshTokenExpiration),
            UserAgent = userAgent,
            IsUsed = false,
            Active = true,
            CreatedDatetimeUtc = utcNow,
            UpdatedDatetimeUtc = utcNow,
        };
    }

    private static string GenerateRefreshTokenString(string refreshTokenId)
    {
        var randomNumber = new byte[32];

        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(randomNumber);

        return $"{refreshTokenId}_{Convert.ToBase64String(randomNumber)}";
    }

    private static string ComputeRefreshTokenId(string refreshToken)
    {
        var temp = refreshToken.Split('_', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (temp.Length != 2)
            return string.Empty;

        return temp[0];
    }

    #endregion
}