using E_PharmaHub.Models.Enums;
using E_PharmaHub.Models;

namespace E_PharmaHub.Services.NotificationServ
{
    public interface INotificationService
    {
        Task<Notification> CreateAndSendAsync(
            string userId,
            string title,
            string message,
            NotificationType type
            );

        Task<object> GetUserNotificationsByCategoryAsync(
            string userId,
            string role);

    }
}
