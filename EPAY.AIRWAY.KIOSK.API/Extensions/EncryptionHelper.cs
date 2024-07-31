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
    private static readonly int GCM_IV_NONCE_SIZE_BYTES = 12;
    private static readonly int PBKDF2_SALT_SIZE_BYTES = 32;
    private static readonly int PBKDF2_ITERATIONS = 65536;
    private static readonly SecureRandom random = new SecureRandom();

    private const byte GcmTagSize = 16; // in bytes

    private static readonly string TRANSFORMATION = "AES/GCM/NoPadding";
    private static readonly HashAlgorithmName hashAlg = HashAlgorithmName.SHA256;
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
    public static T DecryptDataForPaymentGateway<T>(this string data, string secretKey) where T : class
    {
        try
        {
            if (string.IsNullOrEmpty(data) || string.IsNullOrEmpty(secretKey))
                throw new MessageResultException("Lỗi trong quá trình gọi dịch vụ PaymentGateway");

            var plainRaw = data.AesDecrypt(secretKey);
            return JsonSerializer.Deserialize<T>(plainRaw);
        }
        catch (Exception ex)
        {
            throw new MessageResultException("Lỗi trong quá trình gọi dịch vụ PaymentGateway", ex);
        }
    }

    /// <summary>
    /// Chức năng: tạo payload khi gọi request tới dịch vụ Epay-Wallet
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="messageId"></param>
    /// <param name="secretKey"></param>
    /// <param name="privateKey"></param>
    /// <returns></returns>
    public static BaseRequest<string> EncryptedDataForPaymentGateway<T>(this T data, DateTime now, string merchantCode, string secretKey, string privateKey) where T : DecryptRequest
    {
        try
        {
            if (data is null || string.IsNullOrEmpty(merchantCode) || string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(privateKey))
                throw new MessageResultException("Lỗi trong quá trình gọi dịch vụ PaymentGateway");

            // Gán thông tin messageType
            string messageType = string.Empty;
            switch (data)
            {
                case LoginRequest:
                    messageType = "token";
                    break;
                case CreateOrderRequest:
                    messageType = "create_order";
                    break;
                case CheckOrderRequest:
                    messageType = "check_status";
                    break;
                default:
                    throw new MessageResultException("Lỗi trong quá trình gọi dịch vụ PaymentGateway");
            }

            // Tạo bản tin mã hoá request
            data.MerchantCode = merchantCode;
            data.TimeRequest = new DateTimeOffset(now).ToUnixTimeSeconds();
            data.MessageType = messageType;

            var jsonRaw = JsonSerializer.Serialize(data);
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
            throw new MessageResultException("Lỗi trong quá trình gọi dịch vụ PaymentGateway", ex);
        }

    }

    /// <summary>
    /// Chức năng: tạo bản tin mã hoá giống với cổng thanh toán
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="now"></param>
    /// <param name="merchantCode"></param>
    /// <param name="secretKey"></param>
    /// <param name="privateKey"></param>
    /// <returns></returns>
    /// <exception cref="MessageResultException"></exception>
    public static BaseResponse<string> EncryptedDataResponseForPaymentGateway<T>(this T data, DateTime now, string merchantCode, string secretKey, string privateKey) where T : DecryptResponse
    {
        try
        {
            if (data is null || string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(privateKey))
                throw new MessageResultException("Lỗi trong quá trình gọi dịch vụ PaymentGateway");

            // Gán thông tin messageType
            string messageType = string.Empty;
            switch (data)
            {
                case RefundResponse:
                    messageType = "refund_response";
                    break;
                default:
                    throw new MessageResultException("Lỗi trong quá trình gọi dịch vụ PaymentGateway");
            }

            // Tạo bản tin mã hoá request
            data.MerchantCode = merchantCode;
            data.TimeResponse = new DateTimeOffset(now).ToUnixTimeMilliseconds();
            data.MessageType = messageType;
            data.TransId = new DateTimeOffset(now).ToUnixTimeMilliseconds().ToString();

            var jsonRaw = JsonSerializer.Serialize(data);
            var cipherText = jsonRaw.AesEncrypt(secretKey);
            var signature = $"{cipherText}{secretKey}".GeneratePaymentGateway(privateKey);

            return new BaseResponse<string>()
            {
                Data = cipherText,
                Signature = signature,
                MerchantCode = merchantCode
            };
        }
        catch (Exception ex)
        {
            throw new MessageResultException("Lỗi trong quá trình gọi dịch vụ PaymentGateway", ex);
        }

    }
    #endregion

    #region Private work
    /// <summary>
    /// Chức năng: mã hoá string bằng AES (hỗ trợ 128-bit)
    /// </summary>
    /// <param name="plainText"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    private static string AesEncrypt(this string plainText, string secretKey)
    {
        if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(plainText))
            return string.Empty;

        byte[] salt = GenerateRandomArray(PBKDF2_SALT_SIZE_BYTES);
        byte[] key = GetKey(secretKey, salt);
        byte[] iv = GenerateRandomArray(GCM_IV_NONCE_SIZE_BYTES);
        var keyParameters = CreateKeyParameters(key, iv, GcmTagSize * 8);
        var cipher = CipherUtilities.GetCipher(TRANSFORMATION);
        cipher.Init(true, keyParameters);

        var plainTextData = Encoding.UTF8.GetBytes(plainText);
        var cipherText = cipher.DoFinal(plainTextData);

        byte[] result = Arrays.CopyOf(salt, GCM_IV_NONCE_SIZE_BYTES + PBKDF2_SALT_SIZE_BYTES + cipherText.Length);
        Array.Copy(iv, 0, result, PBKDF2_SALT_SIZE_BYTES, GCM_IV_NONCE_SIZE_BYTES);
        Array.Copy(cipherText, 0, result, GCM_IV_NONCE_SIZE_BYTES + PBKDF2_SALT_SIZE_BYTES, cipherText.Length);

        return Convert.ToBase64String(result);
    }

    /// <summary>
    /// Chức năng: giải mã string bằng AES (hỗ trợ 128-bit)
    /// </summary>
    /// <param name="cipherText"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    private static string AesDecrypt(this string cipherText, string secretKey)
    {
        if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(cipherText))
            return string.Empty;

        var (encryptedBytes, iv, salt) = UnpackCipherData(cipherText);
        byte[] key = GetKey(secretKey, salt);
        var keyParameters = CreateKeyParameters(key, iv, GcmTagSize * 8);
        var cipher = CipherUtilities.GetCipher(TRANSFORMATION);
        cipher.Init(false, keyParameters);

        var decryptedData = cipher.DoFinal(encryptedBytes);
        return Encoding.UTF8.GetString(decryptedData);
    }

    private static byte[] GenerateRandomArray(int sizeInBytes)
    {
        byte[] randomArray = new byte[sizeInBytes];
        random.NextBytes(randomArray);
        return randomArray;
    }

    private static byte[] GetKey(string secretKey, byte[] salt)
    {
        var key = Rfc2898DeriveBytes.Pbkdf2(secretKey, salt, PBKDF2_ITERATIONS, hashAlg, PBKDF2_SALT_SIZE_BYTES);

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
        byte[] salt = Arrays.CopyOfRange(bytes, 0, PBKDF2_SALT_SIZE_BYTES);
        byte[] iv = Arrays.CopyOfRange(bytes, PBKDF2_SALT_SIZE_BYTES, PBKDF2_SALT_SIZE_BYTES + GCM_IV_NONCE_SIZE_BYTES);
        byte[] encryptedBytes = Arrays.CopyOfRange(bytes, GCM_IV_NONCE_SIZE_BYTES + PBKDF2_SALT_SIZE_BYTES, bytes.Length);

        return (encryptedBytes, iv, salt);
    }
    #endregion

    #endregion
}