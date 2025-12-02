using E_PharmaHub.Models;

namespace E_PharmaHub.Services.AppointmentNotificationScheduleServe
{
    public interface IAppointmentNotificationScheduler
    {
        Task ScheduleAppointmentNotifications(Appointment appointment);

    }
}
