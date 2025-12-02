using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories.NotificationRepo
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId);
    }
}
