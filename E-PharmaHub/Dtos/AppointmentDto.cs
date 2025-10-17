namespace E_PharmaHub.Dtos
{
    public class AppointmentDto
    {
        public string UserId { get; set; }
        public string DoctorId { get; set; }
        public int ClinicId { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
    }
}
