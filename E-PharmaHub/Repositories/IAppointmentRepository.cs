using E_PharmaHub.Models;
using System.Linq.Expressions;

namespace E_PharmaHub.Repositories
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<bool> ExistsAsync(Expression<Func<Appointment, bool>> predicate);

        Task<Appointment> BookAppointmentAsync(Appointment appointment);
        Task<IEnumerable<Appointment>> GetAppointmentsByDoctorIdAsync(string doctorId);
        Task<IEnumerable<Appointment>> GetAppointmentsByUserIdAsync(string userId);
    }
}
