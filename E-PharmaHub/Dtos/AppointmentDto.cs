using E_PharmaHub.Models.Enums;

namespace E_PharmaHub.Dtos
{
    public class AppointmentDto
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int DoctorId { get; set; }
        public string PatientName { get; set; }
        public string PatientPhone { get; set; }
        public int PatientAge { get; set; }
        public Gender PatientGender { get; set;}
        public int ClinicId { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
    }
}
