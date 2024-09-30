using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.CustomHttpClient.Request;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Services;

public interface ICustomHttpClient
{
    /// <summary>
    /// Chức năng: thực hiện http-request
    /// </summary>
    /// <param name="request">Thông tin request</param>
    /// <param name="func">Func xử lí kết quả trả về</param>
    /// <param name="codeMessageWhenException">code-message trả về trong trường hợp gặp exception khi request</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <typeparam name="TReq">Kiểu dữ liệu gọi</typeparam>
    /// <typeparam name="TRes">Kiểu dữ liệu trả về</typeparam>
    /// <returns></returns>
    Task<(CodeMessage codeMessage, TRes? data)> SendAsync<TReq, TRes>(MyHttpRequest<TReq> request,
        Func<HttpResponseMessage, string, (CodeMessage, TRes?)>? func = null,
        CodeMessage codeMessageWhenException = CodeMessage._3005,
        CancellationToken cancellationToken = default);
}