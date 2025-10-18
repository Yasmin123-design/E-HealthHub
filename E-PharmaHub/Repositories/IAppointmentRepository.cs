using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<Appointment> BookAppointmentAsync(Appointment appointment);
        Task<IEnumerable<Appointment>> GetAppointmentsByDoctorIdAsync(string doctorId);
        Task<IEnumerable<Appointment>> GetAppointmentsByUserIdAsync(string userId);
    }
}
