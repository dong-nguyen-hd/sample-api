namespace EPAY.AIRWAY.KIOSK.API.Extensions.AddConfig;

using Controllers.Hubs;
using Microsoft.AspNetCore.Http.Connections;

public static class RelateHub
{
    public static void UseHub(this WebApplication webApplication)
    {
        webApplication.MapHub<NotificationHub>("api/v1/signalr/notification", opt =>
        {
            opt.Transports = HttpTransportType.WebSockets;
        });
    }
}
