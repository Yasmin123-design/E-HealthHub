namespace E_PharmaHub.Dtos
{
    public class AppointmentDto
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int DoctorId { get; set; }
        public int ClinicId { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
    }
}
