using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using System.Linq.Expressions;

namespace E_PharmaHub.Repositories
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<AppointmentResponseDto> AddAppointmentAndReturnResponseAsync(Appointment appointment);

        Task<bool> ExistsAsync(Expression<Func<Appointment, bool>> predicate);
        Task<AppointmentResponseDto?> GetAppointmentResponseByIdAsync(int id);
        Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsByDoctorIdAsync(string doctorId);
        Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsByUserIdAsync(string userId);
    }
}
