using E_PharmaHub.Dtos;
using E_PharmaHub.UnitOfWorkes;
using System.Globalization;

namespace E_PharmaHub.Services.DoctorAnalyticsServ
{
    public class DoctorAnalyticsService : IDoctorAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DoctorAnalyticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<List<DailyAppointmentsDto>> GetWeeklyAppointmentsAsync(string doctorId) =>
            _unitOfWork.Appointments.GetWeeklyAppointmentsAsync(doctorId);
        public async Task<List<DailyAppointmentsDto>> GetDailyAppointmentsAsync(
     string doctorId,
     int? year,
     int? month)
        {
            var appointments = await _unitOfWork.Appointments
                .GetDailyAppointmentsAsync(doctorId, year, month);

            return appointments;
        }
        public async Task<List<DailyRevenueDto>> GetDailyRevenueAsync(
    string doctorId,
    int? year,
    int? month)
        {
            var revenus = await _unitOfWork.Appointments
                .GetDailyRevenueAsync(doctorId, year, month);

            return revenus;
        }
        public Task<List<DailyRevenueDto>> GetWeeklyRevenueAsync(string doctorId) =>
            _unitOfWork.Appointments.GetWeeklyRevenueAsync(doctorId);

        public Task<GenderStatsDto> GetGenderStatsAsync(string doctorId) =>
            _unitOfWork.Appointments.GetGenderStatsAsync(doctorId);

        public Task<List<DailyStatusStatsDto>> GetWeeklyStatusStatsAsync(string doctorId) =>
            _unitOfWork.Appointments.GetWeeklyStatusStatsAsync(doctorId);

        public Task<List<AgeRangeDto>> GetAgeRangesAsync(string doctorId) =>
            _unitOfWork.Appointments.GetAgeRangesAsync(doctorId);

        public async Task<DoctorDashboardStatsDto> GetDashboardStatsAsync(string doctorId)
        {
            var doctor = await _unitOfWork.Doctors.GetDoctorByUserIdAsync(doctorId);
            return new DoctorDashboardStatsDto
            {
                TodayAppointmentsCount =
                    await _unitOfWork.Appointments.GetTodayAppointmentsCountAsync(doctorId),
                TotalConfirmedAppointmentCount =
                    await _unitOfWork.Appointments.GetTotalConfirmedAppointmentsCountAsync(doctorId),
                TotalCancelledAppointmentCount =
                    await _unitOfWork.Appointments.GetTotalCancelledAppointmentsCountAsync(doctorId),
                TotalCompletedAppointmentCount =
                    await _unitOfWork.Appointments.GetTotalCompletedAppointmentsCountAsync(doctorId),
                TotalPenddingAppointmentCount  =
                    await _unitOfWork.Appointments.GetTotalPenddingAppointmentsCountAsync(doctorId),

                YesterdayAppointmentsCount =
                await _unitOfWork.Appointments.GetYesterdayAppointmentsCountAsync(doctorId),

                YesterdayRevenue =
                await _unitOfWork.Appointments.GetYesterRevenueAsync(doctorId),
                TotalPatientsCount =
                    await _unitOfWork.Appointments.GetTotalPatientsCountAsync(doctorId),

                TodayRevenue =
                    await _unitOfWork.Appointments.GetTodayRevenueAsync(doctorId),

                TotalRevenue =
                    await _unitOfWork.Appointments.GetTotalRevenueAsync(doctorId),

                ReviewsCount =
                    await _unitOfWork.Reviews.GetReviewsCountAsync(doctor.Id)
            };
        }
        public async Task<List<WeeklyPatientsStatsDto>> GetWeeklyPatientsStatsAsync(string doctorId)
        {
            var appointments = await _unitOfWork.Appointments
                .GetConfirmedAppointmentsByDoctorIdAsync(doctorId);

            if (!appointments.Any())
                return new List<WeeklyPatientsStatsDto>();

            var result = appointments
                .GroupBy(a => ISOWeek.GetWeekOfYear(a.StartAt), a => a)
                .Select(g =>
                {
                    var weekAppointments = g.ToList();
                    var weekStart = FirstDateOfWeekISO8601(weekAppointments.First().StartAt.Year, g.Key);

                    var patientGroups = weekAppointments.GroupBy(a => a.UserId).ToList();

                    int newPatients = patientGroups.Count(pg =>
                        appointments
                            .Where(a => a.UserId == pg.Key)
                            .Min(a => a.StartAt) >= weekStart);

                    int returningPatients = patientGroups.Count - newPatients;

                    return new WeeklyPatientsStatsDto
                    {
                        WeekStart = weekStart.ToString("yyyy-MM-dd"),
                        NewPatients = newPatients,
                        ReturningPatients = returningPatients
                    };
                })
                .OrderBy(r => r.WeekStart)
                .ToList();

            return result;
        }

        private DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Monday - jan1.DayOfWeek;

            DateTime firstMonday = jan1.AddDays(daysOffset);
            var cal = System.Globalization.CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(jan1, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            if (firstWeek <= 1)
                weekOfYear -= 1;

            return firstMonday.AddDays(weekOfYear * 7);
        }

    }

}
