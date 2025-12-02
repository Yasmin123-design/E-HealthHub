using E_PharmaHub.Models.Enums;
using E_PharmaHub.Models;
using E_PharmaHub.Services.NotificationServ;
using Hangfire;
namespace E_PharmaHub.Services.AppointmentNotificationScheduleServe
{

    public class AppointmentNotificationScheduler : IAppointmentNotificationScheduler
    {
        public async Task ScheduleAppointmentNotifications(Appointment appointment)
        {
            var appointmentTime = appointment.StartAt;
            var userId = appointment.UserId;

            var reminderTime = appointmentTime.AddHours(-24);

            if (reminderTime > DateTime.Now)
            {
                BackgroundJob.Schedule<INotificationService>(service =>
                    service.CreateAndSendAsync(
                        userId,
                        "Appointment Reminder",
                        $"Your appointment with Dr,{appointment.Doctor.UserName} is in 24 hours",
                        NotificationType.AppointmentReminder
                    ),
                    reminderTime
                );
            }

            var soonTime = appointmentTime.AddMinutes(-10);

            if (soonTime > DateTime.Now)
            {
                BackgroundJob.Schedule<INotificationService>(service =>
                    service.CreateAndSendAsync(
                        userId,
                        "Appointment Starting Soon",
                        $"Your appointment with Dr.{appointment.Doctor.UserName} starts in 10 minutes, join now",
                        NotificationType.AppointmentStartingSoon
                    ),
                    soonTime
                );
            }
        }
    }

}
