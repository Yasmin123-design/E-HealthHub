using Microsoft.AspNetCore.SignalR;

namespace E_PharmaHub.Services.UserIdProviderServ
{
    public interface IUserIdProvider
    {
        string GetUserId(HubConnectionContext connection);
    }
}
