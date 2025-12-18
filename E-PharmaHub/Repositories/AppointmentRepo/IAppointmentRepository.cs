using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using System.Linq.Expressions;

namespace E_PharmaHub.Repositories.AppointmentRepo
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<Appointment> GetAppointmentByPaymentIdAsync(int paymentId);
        Task<IEnumerable<AppointmentResponseDto>> GetByStatusAsync(
    AppointmentStatus status);
        Task<int> GetTotalAppointmentsCountAsync(string doctorId);
        Task<List<Appointment>> GetPatientsOfDoctorAsync(string doctorId);
        Task<int> GetTodayAppointmentsCountAsync(string doctorId);
        Task<int> GetTotalPatientsCountAsync(string doctorId);
        Task<decimal> GetTodayRevenueAsync(string doctorId);
        Task<decimal> GetTotalRevenueAsync(string doctorId);
        Task<AppointmentResponseDto> AddAppointmentAndReturnResponseAsync(Appointment appointment);

        Task<bool> ExistsAsync(Expression<Func<Appointment, bool>> predicate);
        Task<AppointmentResponseDto?> GetAppointmentResponseByIdAsync(int id);
        Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsByDoctorIdAsync(string doctorId);
        Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsByUserIdAsync(string userId);
    }
}
