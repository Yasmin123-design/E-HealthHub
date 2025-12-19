using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace E_PharmaHub.Repositories.NotificationRepo
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly EHealthDbContext _context;

        public NotificationRepository(EHealthDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
        }

        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }
        public async Task<List<Notification>> GetUnreadByUserAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();
        }
        public async Task<Notification?> GetByIdAsync(int id)
        {
            return await _context.Notifications.FindAsync(id);
        }

        public void Update(Notification notification)
        {
            _context.Notifications.Update(notification);
        }
    }

}
