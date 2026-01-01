using E_PharmaHub.Dtos;

namespace E_PharmaHub.Services.DoctorAnalyticsServ
{
    public interface IDoctorAnalyticsService
    {
        Task<List<DailyRevenueDto>> GetDailyRevenueAsync(
     string doctorId,
     int? year,
     int? month);

        Task<List<DailyAppointmentsDto>> GetDailyAppointmentsAsync(
            string doctorId,
            int? year,
            int? month);
        Task<List<WeeklyPatientsStatsDto>> GetWeeklyPatientsStatsAsync(string doctorId);
        Task<DoctorDashboardStatsDto> GetDashboardStatsAsync(string doctorId);
        Task<List<DailyAppointmentsDto>> GetWeeklyAppointmentsAsync(string doctorId);
        Task<List<DailyRevenueDto>> GetWeeklyRevenueAsync(string doctorId);
        Task<GenderStatsDto> GetGenderStatsAsync(string doctorId);
        Task<List<DailyStatusStatsDto>> GetWeeklyStatusStatsAsync(string doctorId);
        Task<List<AgeRangeDto>> GetAgeRangesAsync(string doctorId);


    }
}
