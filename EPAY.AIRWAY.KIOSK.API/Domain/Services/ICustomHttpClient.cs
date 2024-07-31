using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.CustomHttpClient.Request;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Services;

public interface ICustomHttpClient
{
    Task<(bool isSuccess, TRes? data)> SendAsync<TRes>(MyHttpRequest request, CancellationToken cancellationToken);
}