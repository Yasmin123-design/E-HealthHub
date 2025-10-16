using Microsoft.AspNetCore.SignalR;

namespace E_PharmaHub.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string threadId, string senderId, string message)
        {
            await Clients.Group(threadId).SendAsync("ReceiveMessage", senderId, message, DateTime.UtcNow);
        }

        public override async Task OnConnectedAsync()
        {
            var threadId = Context.GetHttpContext()?.Request.Query["threadId"];
            if (!string.IsNullOrEmpty(threadId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, threadId);
            }
            await base.OnConnectedAsync();
        }
    }
}
