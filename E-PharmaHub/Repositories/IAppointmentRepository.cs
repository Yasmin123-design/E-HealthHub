using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories
{
    public interface IAppointmentRepository
    {
        Task<Appointment> BookAppointmentAsync(Appointment appointment);
    }
}
