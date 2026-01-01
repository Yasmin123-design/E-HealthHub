namespace E_PharmaHub.Dtos
{
    public class DoctorDashboardStatsDto
    {
        public int TodayAppointmentsCount { get; set; }
        public int YesterdayAppointmentsCount { get; set; }
        public decimal YesterdayRevenue { get; set; }
        public int TotalPenddingAppointmentCount { get; set; }
        public int TotalConfirmedAppointmentCount { get; set; }
        public int TotalCancelledAppointmentCount { get; set; }
        public int TotalCompletedAppointmentCount { get; set; }

        public int TotalPatientsCount { get; set; }
        public decimal TodayRevenue { get; set; }
        public decimal TotalRevenue { get; set; }
        public int ReviewsCount { get; set; }
    }
}
