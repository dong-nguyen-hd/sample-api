using System.Security.Cryptography;
using System.Text;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.PaymentGateway.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.PaymentGateway.Response;
using EPAY.AIRWAY.KIOSK.API.Resources.Exceptions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;

namespace EPAY.AIRWAY.KIOSK.API.Extensions;

public static class EncryptionHelper
{
    #region Properties

    // AES
    private static readonly int _gcmIvNonceSizeBytes = 12;
    private static readonly int _pbkdf2SaltSizeBytes = 32;
    private static readonly int _pbkdf2Iterations = 65536;
    private static readonly SecureRandom _random = new();

    private const byte GcmTagSize = 16; // in bytes

    private static readonly string _transformation = "AES/GCM/NoPadding";
    private static readonly HashAlgorithmName _hashAlg = HashAlgorithmName.SHA256;

    #endregion

    #region Method

    #region Payment gateway

    /// <summary>
    /// Chức năng: giải mã data phản hồi từ cổng thanh toán
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="secretKey"></param>
    /// <returns></returns>
    /// <exception cref="MessageResultException"></exception>
    public static T? DecryptDataForPaymentGateway<T>(this string? data, string? secretKey)
    {
        try
        {
            if (string.IsNullOrEmpty(data) || string.IsNullOrEmpty(secretKey))
                throw new MessageResultException("[1] Lỗi giải mã bản tin PaymentGateway");

            var plainRaw = data.AesDecrypt(secretKey);
            return JsonSerializer.Deserialize<T>(plainRaw);
        }
        catch (Exception ex)
        {
            throw new MessageResultException($"[2] Lỗi giải mã bản tin PaymentGateway: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Chức năng: tạo payload khi gọi request tới dịch vụ Epay-Wallet
    /// </summary>
    /// <param name="data"></param>
    /// <param name="now"></param>
    /// <param name="merchantCode"></param>
    /// <param name="secretKey"></param>
    /// <param name="privateKey"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="MessageResultException"></exception>
    public static BaseRequest<string> EncryptedDataForPaymentGateway<T>(this T data, DateTime now, string merchantCode, string secretKey, string privateKey) where T : DecryptRequest
    {
        try
        {
            if (data is null ||
                string.IsNullOrEmpty(merchantCode) ||
                string.IsNullOrEmpty(secretKey) ||
                string.IsNullOrEmpty(privateKey))
                throw new MessageResultException("[1] Lỗi mã hoá bản tin PaymentGateway");

            // Gán thông tin messageType
            switch (data)
            {
                case LoginRequest:
                    data.MessageType = "token";
                    break;
                case CreateOrderRequest:
                    data.MessageType = "create_order";
                    break;
                case CheckOrderRequest:
                    data.MessageType = "check_status";
                    break;
                default:
                    throw new MessageResultException("[2] Lỗi mã hoá bản tin PaymentGateway");
            }

            // Tạo bản tin mã hoá request
            data.MerchantCode = merchantCode;
            data.TimeRequest = new DateTimeOffset(now).ToUnixTimeSeconds();

            var jsonRaw = data.MySerialize();
            var cipherText = jsonRaw.AesEncrypt(secretKey);
            var signature = $"{cipherText}{secretKey}".GeneratePaymentGateway(privateKey);

            return new BaseRequest<string>()
            {
                Data = cipherText,
                Signature = signature,
                MerchantCode = merchantCode
            };
        }
        catch (Exception ex)
        {
            throw new MessageResultException($"[3] Lỗi mã hoá bản tin PaymentGateway: {ex.Message}", ex);
        }
    }

    #endregion

    #region My Aes

    /// <summary>
    /// Chức năng: mã hoá string bằng AES (hỗ trợ 128-bit)
    /// </summary>
    /// <param name="plainText"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string MyAesEncrypt(this string? plainText, string? key)
    {
        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(plainText))
            return string.Empty;

        byte[] iv = new byte[16];
        byte[] array;

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                    {
                        streamWriter.Write(plainText);
                    }

                    array = memoryStream.ToArray();
                }
            }
        }

        return Convert.ToBase64String(array);
    }

    /// <summary>
    /// Chức năng: giải mã string bằng AES (hỗ trợ 128-bit)
    /// </summary>
    /// <param name="cipherText"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string MyAesDecrypt(this string? cipherText, string? key)
    {
        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(cipherText))
            return string.Empty;

        byte[] iv = new byte[16];
        byte[] buffer = Convert.FromBase64String(cipherText);

        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream(buffer))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader streamReader = new StreamReader(cryptoStream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
        }
    }

    #endregion

    #region Private work

    /// <summary>
    /// Chức năng: mã hoá string bằng AES (hỗ trợ 128-bit)
    /// </summary>
    /// <param name="plainText"></param>
    /// <param name="secretKey"></param>
    /// <returns></returns>
    private static string AesEncrypt(this string plainText, string secretKey)
    {
        if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(plainText))
            return string.Empty;

        byte[] salt = GenerateRandomArray(_pbkdf2SaltSizeBytes);
        byte[] key = GetKey(secretKey, salt);
        byte[] iv = GenerateRandomArray(_gcmIvNonceSizeBytes);
        var keyParameters = CreateKeyParameters(key, iv, GcmTagSize * 8);
        var cipher = CipherUtilities.GetCipher(_transformation);
        cipher.Init(true, keyParameters);

        var plainTextData = Encoding.UTF8.GetBytes(plainText);
        var cipherText = cipher.DoFinal(plainTextData);

        byte[] result = Arrays.CopyOf(salt, _gcmIvNonceSizeBytes + _pbkdf2SaltSizeBytes + cipherText.Length);
        Array.Copy(iv, 0, result, _pbkdf2SaltSizeBytes, _gcmIvNonceSizeBytes);
        Array.Copy(cipherText, 0, result, _gcmIvNonceSizeBytes + _pbkdf2SaltSizeBytes, cipherText.Length);

        return Convert.ToBase64String(result);
    }

    /// <summary>
    /// Chức năng: giải mã string bằng AES (hỗ trợ 128-bit)
    /// </summary>
    /// <param name="cipherText"></param>
    /// <param name="secretKey"></param>
    /// <returns></returns>
    private static string AesDecrypt(this string cipherText, string secretKey)
    {
        if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(cipherText))
            return string.Empty;

        var (encryptedBytes, iv, salt) = UnpackCipherData(cipherText);
        byte[] key = GetKey(secretKey, salt);
        var keyParameters = CreateKeyParameters(key, iv, GcmTagSize * 8);
        var cipher = CipherUtilities.GetCipher(_transformation);
        cipher.Init(false, keyParameters);

        var decryptedData = cipher.DoFinal(encryptedBytes);
        return Encoding.UTF8.GetString(decryptedData);
    }

    private static byte[] GenerateRandomArray(int sizeInBytes)
    {
        byte[] randomArray = new byte[sizeInBytes];
        _random.NextBytes(randomArray);
        return randomArray;
    }

    private static byte[] GetKey(string secretKey, byte[] salt)
    {
        var key = Rfc2898DeriveBytes.Pbkdf2(secretKey, salt, _pbkdf2Iterations, _hashAlg, _pbkdf2SaltSizeBytes);

        return key;
    }

    private static ICipherParameters CreateKeyParameters(byte[] key, byte[] iv, int macSize)
    {
        var keyParameter = new KeyParameter(key);
        return new AeadParameters(keyParameter, macSize, iv);
    }

    private static (byte[], byte[], byte[]) UnpackCipherData(string cipherText)
    {
        byte[] bytes = Convert.FromBase64String(cipherText);
        byte[] salt = Arrays.CopyOfRange(bytes, 0, _pbkdf2SaltSizeBytes);
        byte[] iv = Arrays.CopyOfRange(bytes, _pbkdf2SaltSizeBytes, _pbkdf2SaltSizeBytes + _gcmIvNonceSizeBytes);
        byte[] encryptedBytes = Arrays.CopyOfRange(bytes, _gcmIvNonceSizeBytes + _pbkdf2SaltSizeBytes, bytes.Length);

        return (encryptedBytes, iv, salt);
    }

    #endregion

    #endregion
}