using EPAY.AIRWAY.KIOSK.API.Controllers.Hubs;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using Microsoft.AspNetCore.SignalR;

namespace EPAY.AIRWAY.KIOSK.API.Services;

public sealed class SignalRService(IHubContext<NotificationHub> hubContext) : BaseService, ISignalRService
{
    public async Task PublicMessage<T>(T? obj) where T : class, new()
    {
        if (obj is null)
            return;

        string message = JsonSerializer.Serialize(obj);
        await hubContext.Clients.All.SendAsync("notification", message);
    }
}