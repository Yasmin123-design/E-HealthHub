using E_PharmaHub.Hubs;
using E_PharmaHub.Models.Enums;
using E_PharmaHub.Models;
using E_PharmaHub.UnitOfWorkes;
using Microsoft.AspNetCore.SignalR;

namespace E_PharmaHub.Services.NotificationServ
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<NotificationHub> _hub;

        public NotificationService(IUnitOfWork unitOfWork, IHubContext<NotificationHub> hub)
        {
            _unitOfWork = unitOfWork;
            _hub = hub;
        }

        public async Task<Notification> CreateAndSendAsync(
            string userId,
            string title,
            string message,
            NotificationType type
            )
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
            };

            await _unitOfWork.Notifications.AddAsync(notification);

            await _unitOfWork.CompleteAsync();

            await _hub.Clients.User(userId).SendAsync("ReceiveNotification", new
            {
                id = notification.Id,
                title = notification.Title,
                message = notification.Message,
                type = notification.Type.ToString(),
                createdAt = notification.CreatedAt
            });

            return notification;
        }

        public async Task SendAppointmentNotificationIfValidAsync(
    int appointmentId,
    string userId,
    string title,
    string message,
    NotificationType type)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId);

            if (appointment == null)
                return;

            if (appointment.Status != AppointmentStatus.Confirmed)
                return; 

            await CreateAndSendAsync(userId, title, message, type);
        }

        public async Task<object> GetUserNotificationsByCategoryAsync(
    string userId,
    string role)
        {
            var notifications = await _unitOfWork.Notifications
                .GetUserNotificationsAsync(userId);

            if (role == "Doctor")
            {
                return new
                {
                    AppointmentRequests = notifications
                };
            }

            var orders = notifications
                .Where(n => n.Type == NotificationType.OrderCancelled
                         || n.Type == NotificationType.OrderConfirmed
                         || n.Type == NotificationType.OrderDelivered);

            var appointments = notifications
                .Where(n => n.Type == NotificationType.AppointmentApproved
                         || n.Type == NotificationType.AppointmentRejected
                         || n.Type == NotificationType.AppointmentReminder
                         || n.Type == NotificationType.AppointmentStartingSoon);

            return new
            {
                Orders = orders,
                Appointments = appointments
            };
        }

        public async Task MarkAsReadAsync(int notificationId, string userId)
        {
            var notification =
                await _unitOfWork.Notifications.GetByIdAsync(notificationId);

            if (notification == null)
                throw new Exception("Notification not found");

            if (notification.UserId != userId)
                throw new Exception("Unauthorized access");

            notification.IsRead = true;

            _unitOfWork.Notifications.Update(notification);
            await _unitOfWork.CompleteAsync();
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            var notifications =
                await _unitOfWork.Notifications.GetUnreadByUserAsync(userId);

            foreach (var notification in notifications)
                notification.IsRead = true;

            await _unitOfWork.CompleteAsync();
        }


    }
}
