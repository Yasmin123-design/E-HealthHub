using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace E_PharmaHub.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
    }
}
