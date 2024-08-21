namespace EPAY.AIRWAY.KIOSK.API.Extensions;

using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

public static class HashingHelper
{
    /// <summary>
    /// Chức năng: so khớp mã hash hợp lệ?
    /// </summary>
    /// <param name="hashStorage">Mã hash lưu tại CSDL dùng để đối chiếu</param>
    /// <param name="hashVerify">Mã hash cần kiểm tra</param>
    /// <returns></returns>
    public static bool CheckingPassword(this string hashStorage, string hashVerify)
    {
        try
        {
            string[] arrPassword = hashStorage.Split('.');
            if (arrPassword.Length != 3)
                return false;

            string hashingPwd = GetHashPBKDF2(hashVerify,
                Convert.FromBase64String(arrPassword[1]),
                Convert.ToInt32(arrPassword[0]));

            if (string.Equals(hashingPwd, arrPassword[2]))
                return true;

            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Chức năng: Hashing plain text with PBKDF2
    /// </summary>
    /// <param name="plainText"></param>
    /// <param name="iterationCount"></param>
    /// <returns></returns>
    public static string HashingPassword(this string plainText, int iterationCount = 10000)
    {
        // Generate a 128 - bit salt using a secure PRNG
        byte[] salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
            rng.GetBytes(salt);

        string hashed = GetHashPBKDF2(plainText, salt, iterationCount);

        return string.Format($"{iterationCount}.{Convert.ToBase64String(salt)}.{hashed}"); // Format: {iterationCount}.{salt}.{hash}
    }

    #region Private work

    /// <summary>
    /// Chức năng: tạo mã hash <br/>
    /// <seealso href="https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-6.0">Tìm hiểu thêm tại đây</seealso>
    /// </summary>
    /// <param name="plainText"></param>
    /// <param name="salt"></param>
    /// <param name="iterationCount"></param>
    /// <returns></returns>
    private static string GetHashPBKDF2(string plainText, byte[] salt, int iterationCount)
    {
        // Derive a 256-bit subkey (use HMAC-SHA512 with 10,000 iterations is default)
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: plainText.ToLower(),
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: iterationCount,
            numBytesRequested: 256 / 8));

        return hashed;
    }

    #endregion
}