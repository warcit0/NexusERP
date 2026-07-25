using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace NexusERP.API.Hubs;

public class NotificationHub : Hub
{
    // Método que el cliente puede invocar (opcional, pero útil)
    public async Task SendNotification(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveNotification", user, message);
    }
}
