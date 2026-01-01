using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.Models.Enums;
using System.Linq.Expressions;

namespace E_PharmaHub.Repositories.AppointmentRepo
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<List<DailyStatusStatsDto>> GetWeeklyStatusStatsAsync(string doctorId);
        Task<List<AgeRangeDto>> GetAgeRangesAsync(string doctorId);
        Task<GenderStatsDto> GetGenderStatsAsync(string doctorId);
        Task<List<DailyRevenueDto>> GetWeeklyRevenueAsync(string doctorId);
        Task<List<DailyAppointmentsDto>> GetWeeklyAppointmentsAsync(string doctorId);

        Task<IEnumerable<Appointment>> GetBookedByDoctorAndDateAsync(
       int doctorId,
       DateTime date);
        Task<IEnumerable<AppointmentResponseDto>> GetConfirmedAppointmentsByDoctorIdAsync(string doctorId);

        Task<bool> IsSlotBookedAsync(
            int doctorId,
            DateTime startAt,
            DateTime endAt);
        Task<int> GetYesterdayAppointmentsCountAsync(string doctorId);
        Task<decimal> GetYesterRevenueAsync(string doctorId);
        Task<List<DailyRevenueDto>> GetDailyRevenueAsync(
    string doctorId,
    int? year,
    int? month);
        Task<List<DailyAppointmentsDto>> GetDailyAppointmentsAsync(
   string doctorId,
   int? year,
   int? month);
        Task<Appointment> GetAppointmentByPaymentIdAsync(int paymentId);
        Task<IEnumerable<AppointmentResponseDto>> GetByStatusAsync(
    AppointmentStatus status);
        Task<int> GetTotalConfirmedAppointmentsCountAsync(string doctorId);
        Task<int> GetTotalCancelledAppointmentsCountAsync(string doctorId);
        Task<int> GetTotalCompletedAppointmentsCountAsync(string doctorId);
        Task<int> GetTotalPenddingAppointmentsCountAsync(string doctorId);

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
