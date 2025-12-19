using E_PharmaHub.Models;
using E_PharmaHub.Models.Enums;
using E_PharmaHub.Services.NotificationServ;
using Hangfire;

namespace E_PharmaHub.Services.AppointmentNotificationScheduleServe
{
    public class AppointmentNotificationScheduler : IAppointmentNotificationScheduler
    {
        public async Task ScheduleAppointmentNotifications(Appointment appointment)
        {
            var appointmentUtc = DateTime.SpecifyKind(
                appointment.StartAt,
                DateTimeKind.Utc
            );

            var userId = appointment.UserId;
            var doctorId = appointment.DoctorId;
            var nowUtc = DateTime.UtcNow;

            var reminderUtc = appointmentUtc.AddHours(-24);

            if (reminderUtc > nowUtc)
            {
                BackgroundJob.Schedule<INotificationService>(
                    service => service.SendAppointmentNotificationIfValidAsync(
                        appointment.Id,
                        userId,
                        "Appointment Reminder",
                        $"Your appointment with Dr.{appointment.Doctor.UserName} is in 24 hours",
                        NotificationType.AppointmentReminder
                    ),
                    reminderUtc
                );
                BackgroundJob.Schedule<INotificationService>(
                   service => service.SendAppointmentNotificationIfValidAsync(
                       appointment.Id,
                       doctorId,
                       "Appointment Reminder",
                       $"Your appointment with patient.{appointment.PatientName} is in 24 hours",
                       NotificationType.AppointmentReminder
                   ),
                   reminderUtc
               );
            }

            var soonUtc = appointmentUtc.AddMinutes(-10);

            if (soonUtc > nowUtc)
            {
                BackgroundJob.Schedule<INotificationService>(
                    service => service.SendAppointmentNotificationIfValidAsync(
                        appointment.Id,
                        userId,
                        "Appointment Starting Soon",
                        $"Your appointment with Dr. {appointment.Doctor.UserName} starts in 10 minutes, join now",
                        NotificationType.AppointmentStartingSoon
                    ),
                    soonUtc
                );
                BackgroundJob.Schedule<INotificationService>(
                    service => service.SendAppointmentNotificationIfValidAsync(
                        appointment.Id,
                        doctorId,
                        "Appointment Starting Soon",
                        $"Your appointment with patient.{appointment.PatientName} starts in 10 minutes, join now",
                        NotificationType.AppointmentStartingSoon
                    ),
                    soonUtc
                );
            }
        }
    }
}
