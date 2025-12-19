using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories.NotificationRepo
{
    public interface INotificationRepository
    {
        Task<Notification?> GetByIdAsync(int id);
        void Update(Notification notification);
        Task<List<Notification>> GetUnreadByUserAsync(string userId);
        Task AddAsync(Notification notification);
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId);
    }
}
