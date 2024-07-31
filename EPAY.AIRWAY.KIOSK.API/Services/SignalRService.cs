namespace EPAY.AIRWAY.KIOSK.API.Services;

using Controllers.Hubs;
using Domain.Context;
using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using Microsoft.AspNetCore.SignalR;

public sealed class SignalRService(IMapper mapper,
    CoreContext context,
    IHubContext<NotificationHub> hubContext) : BaseService(mapper, context), ISignalRService
{
    #region Method
    public async Task PublicMessage<T>(T obj) where T : class, new()
    {
        if (obj is null)
            return;

        string message = JsonSerializer.Serialize(obj);
        await hubContext.Clients.All.SendAsync("notification", message);
    }
    #endregion 
}
