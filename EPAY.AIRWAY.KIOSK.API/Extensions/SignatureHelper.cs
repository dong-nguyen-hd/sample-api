using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace EPAY.AIRWAY.KIOSK.API.Extensions;

public static class SignatureHelper
{
    #region Method

    #region Epay wallet
    public static string GenerateEpayWallet(this string data, string privateKey)
    {
        var pemReader = new PemReader(new StringReader(privateKey));

        var rsaPrivateCrtKeyParameters = (RsaPrivateCrtKeyParameters)pemReader.ReadObject();
        ISigner signer = SignerUtilities.GetSigner("SHA256WITHRSA");

        signer.Init(true, rsaPrivateCrtKeyParameters);

        var bytes = Encoding.UTF8.GetBytes(data);

        signer.BlockUpdate(bytes, 0, bytes.Length);
        byte[] signature = signer.GenerateSignature();

        return Convert.ToBase64String(signature);
    }

    public static bool VerifyEpayWallet(this string data, string dataSigned, string publicKey)
    {
        var pemReader = new PemReader(new StringReader(publicKey));
        RsaKeyParameters rsaKeyParameters = (RsaKeyParameters)pemReader.ReadObject();

        ISigner signer = SignerUtilities.GetSigner("SHA256WITHRSA");

        signer.Init(false, rsaKeyParameters);

        var expectedSig = Convert.FromBase64String(dataSigned);

        var msgBytes = Encoding.UTF8.GetBytes(data);

        signer.BlockUpdate(msgBytes, 0, msgBytes.Length);
        return signer.VerifySignature(expectedSig);
    }
    #endregion

    #region RSA Signature
    /// <summary>
    /// Chức năng: kí cho request Qr-Gateway
    /// </summary>
    /// <param name="data"></param>
    /// <param name="privateKey"></param>
    /// <returns></returns>
    public static string GeneratePaymentGateway(this string data, string privateKey)
    {
        byte[] decoded = Convert.FromBase64String(privateKey.RemoveAllSpaceChar());

        AsymmetricKeyParameter asymmetricKeyParameter = PrivateKeyFactory.CreateKey(decoded);
        RsaKeyParameters rsaKeyParameters = (RsaKeyParameters)asymmetricKeyParameter;

        ISigner signer = SignerUtilities.GetSigner("SHA1withRSA");

        signer.Init(true, rsaKeyParameters);

        var bytes = Encoding.UTF8.GetBytes(data);

        signer.BlockUpdate(bytes, 0, bytes.Length);
        byte[] signature = signer.GenerateSignature();

        return Convert.ToBase64String(signature);
    }

    /// <summary>
    /// Chức năng: kiểm tra dữ liệu signature Qr-Gateway hợp lệ
    /// </summary>
    /// <param name="data"></param>
    /// <param name="dataSigned"></param>
    /// <param name="publicKey"></param>
    /// <returns></returns>
    public static bool VerifyPaymentGateway(this string data, string dataSigned, string publicKey)
    {
        byte[] decoded = Convert.FromBase64String(publicKey.RemoveAllSpaceChar());

        AsymmetricKeyParameter asymmetricKeyParameter = PublicKeyFactory.CreateKey(decoded);
        RsaKeyParameters rsaKeyParameters = (RsaKeyParameters)asymmetricKeyParameter;

        ISigner signer = SignerUtilities.GetSigner("SHA1withRSA");

        signer.Init(false, rsaKeyParameters);

        var expectedSig = Convert.FromBase64String(dataSigned);

        var msgBytes = Encoding.UTF8.GetBytes(data);

        signer.BlockUpdate(msgBytes, 0, msgBytes.Length);
        return signer.VerifySignature(expectedSig);
    }
    #endregion

    #endregion
}