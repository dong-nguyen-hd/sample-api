using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using EPAY.AIRWAY.KIOSK.API.Domain.Context;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Authentication.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Authentication.Response;
using EPAY.AIRWAY.KIOSK.API.Resources.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EPAY.AIRWAY.KIOSK.API.Services;

public sealed class TokenManagementService(IMapper mapper,
    IConfigurationService configurationService,
    CoreContext context) : BaseService, ITokenManagementService
{
    #region Properties

    private string _hostBE = string.Empty;

    #endregion
    
    public async Task<BaseResult<TokenResponse>> GenerateNewTokensAsync(RefreshTokenRequest refreshTokenRequest, DateTime utcNow, CancellationToken cancellationToken = default)
    {
        // Trích xuất thông tin về refreshTokenId từ chuỗi refreshToken
        var oldRefreshToken = refreshTokenRequest.RefreshToken.ComputeRefreshTokenId();
        if (string.IsNullOrEmpty(oldRefreshToken.id))
            return GetBaseResult<TokenResponse>(CodeMessage._4002);

        // Xác thực refreshToken
        var refreshTokenDb = await context.RefreshTokens
            .Include(x => x.Account)
            .SingleOrDefaultAsync(x => x.Id == oldRefreshToken.id, cancellationToken);

        if (refreshTokenDb == null ||
            refreshTokenDb.Token != refreshTokenRequest.RefreshToken ||
            refreshTokenDb.IsUsed ||
            !(DateTime.Compare(refreshTokenDb.ExpiredUtc, utcNow) > 0))
            return GetBaseResult<TokenResponse>(CodeMessage._4002);

        var accountDb = refreshTokenDb.Account;

        // Tạo access-token
        var accessToken = GenerateAccessToken(accountDb, utcNow);

        // Tạo refresh-token
        var newRefreshToken = GenerateRefreshToken(utcNow, refreshTokenRequest.UserAgent);
        newRefreshToken.AccountId = accountDb.Id;

        // Gán giá trị cho hoạt động cuối
        accountDb.UpdatedDatetimeUtc = DateTime.UtcNow;
        context.Update(accountDb);

        // Vô hiệu refresh-token cũ
        refreshTokenDb.IsUsed = true;
        context.RefreshTokens.Update(refreshTokenDb);

        // Thêm mới một refresh-token
        await context.AddAsync(newRefreshToken, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var result = new TokenResponse
        {
            Id = newRefreshToken.Id,
            AccessToken = accessToken.value,
            AccessTokenExpireTimeUTC = accessToken.expiredTime,
            RefreshTokenExpireTimeUTC = newRefreshToken.ExpiredUtc,
            RefreshToken = newRefreshToken.Token,
        };

        return GetBaseResult(CodeMessage._0000, data: result);
    }

    public async Task<BaseResult<bool>> LogoutAsync(LogoutRequest logoutRequest, CancellationToken cancellationToken = default)
    {
        // Trích xuất thông tin về refreshTokenId từ chuỗi refreshToken
        var oldRefreshToken = logoutRequest.RefreshToken.ComputeRefreshTokenId();
        if (string.IsNullOrEmpty(oldRefreshToken.id))
            return GetBaseResult<bool>(CodeMessage._4002);

        // Xác thực refresh-token
        var refreshTokenDb = await context.RefreshTokens.SingleOrDefaultAsync(x => x.Id == oldRefreshToken.id, cancellationToken);
        if (refreshTokenDb == null || refreshTokenDb.Token != logoutRequest.RefreshToken)
            return GetBaseResult<bool>(CodeMessage._4002);

        refreshTokenDb.IsUsed = true;
        context.RefreshTokens.Update(refreshTokenDb);
        await context.SaveChangesAsync(cancellationToken);

        return GetBaseResult(CodeMessage._0000, data: true);
    }

    #region Login

    public async Task<BaseResult<AccessTokenResponse>> GenerateTokensAsync(LoginRequest loginRequest, DateTime utcNow, string userAgent, CancellationToken cancellationToken = default)
    {
        await GetConfigDataAsync(cancellationToken);
        
        // Xác thực login-request
        var tempAccount = await context.Accounts
            .AsNoTracking()
            .Select(x => new Model.Account()
            {
                Id = x.Id,
                UserName = x.UserName,
                Password = x.Password
            })
            .SingleOrDefaultAsync(x => x.UserName == loginRequest.UserName, cancellationToken);

        if (tempAccount == null)
            return GetBaseResult<AccessTokenResponse>(CodeMessage._4002);
        bool isValid = tempAccount.Password.CheckingPassword(loginRequest.Password);
        if (!isValid)
            return GetBaseResult<AccessTokenResponse>(CodeMessage._4002);

        // Lấy dữ liệu account sau khi đã xác thực hợp lệ
        var accountDb = await context.Accounts.SingleOrDefaultAsync(x => x.Id == tempAccount.Id, cancellationToken);

        // Lọc theme-type
        if (accountDb == null)
            return GetBaseResult<AccessTokenResponse>(CodeMessage._4002);
        MappingAdditionData(accountDb, loginRequest);

        // Tạo access-token
        var accessToken = GenerateAccessToken(accountDb, utcNow);

        // Tạo refresh-token
        var refreshToken = GenerateRefreshToken(utcNow, userAgent);
        refreshToken.AccountId = accountDb.Id;

        // Gán giá trị cho hoạt động cuối
        accountDb.UpdatedDatetimeUtc = DateTime.UtcNow;
        context.Update(accountDb);

        // Thêm mới một refresh-token
        await context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var dataResult = MappingTokenResoure(accountDb, refreshToken, accessToken.value, accessToken.expiredTime);

        return GetBaseResult(CodeMessage._0000, data: dataResult);
    }

    private void MappingAdditionData(Model.Account account, LoginRequest request)
    {
        if (account.AdditionData == null || request.Type == null)
            return;

        if (account?.AdditionData?.Themes?.Count > 0)
        {
            foreach (var theme in account.AdditionData.Themes)
            {
                if (theme.Type != request.Type)
                {
                    account.AdditionData.Themes.Remove(theme);
                    continue;
                }

                if(theme.PaymentMethods == null)
                    continue;
                foreach (var paymentMethod in theme.PaymentMethods)
                    paymentMethod.Icon = $"{_hostBE}{paymentMethod.Icon}";
            }
        }
    }

    #endregion
    

    #region Private work

    private async Task GetConfigDataAsync(CancellationToken cancellationToken = default)
    {
        // Get config from DB
        var configurations = await configurationService.GetAllAsync(false, cancellationToken);

        if (configurations.CodeMessage != CodeMessage._0000)
            throw new MessageResultException("Không thể thực hiện lấy config");

        foreach (var configuration in configurations.Data!)
        {
            // Config
            if (configuration.Key == SystemConfig.SystemBeHost)
            {
                this._hostBE = configuration.Value!;
                break;
            }
        }
    }
    
    private AccessTokenResponse MappingTokenResoure(Model.Account account, Model.RefreshToken refreshToken, string accessToken, DateTime expiredTime)
    {
        var tokenResponse = mapper.Map<AccessTokenResponse>(account);
        tokenResponse.TokenResponse = mapper.Map<TokenResponse>(refreshToken);
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
        var refreshTokenId = RelateText.GenId();

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

    #endregion
}