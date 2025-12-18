namespace E_PharmaHub.Dtos
{
    public class DoctorDashboardStatsDto
    {
        public int TodayAppointmentsCount { get; set; }
        public int TotalAppointmentCount { get; set; }
        public int TotalPatientsCount { get; set; }
        public decimal TodayRevenue { get; set; }
        public decimal TotalRevenue { get; set; }
        public int ReviewsCount { get; set; }
    }
}
