using EPAY.AIRWAY.KIOSK.API.Controllers.Hubs;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using Microsoft.AspNetCore.SignalR;

namespace EPAY.AIRWAY.KIOSK.API.Services;

public sealed class SignalRService(IHubContext<NotificationHub> hubContext) : BaseService, ISignalRService
{
    public async Task PublicMessageAsync<T>(T? obj, CancellationToken cancellationToken = default) where T : class, new()
    {
        if (obj is null)
            return;

        string message = obj.MySerialize();
        await hubContext.Clients.All.SendAsync("notification", message, cancellationToken);
    }
}