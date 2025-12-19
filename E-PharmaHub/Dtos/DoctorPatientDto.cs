using E_PharmaHub.Models.Enums;
using E_PharmaHub.Models;

namespace E_PharmaHub.Dtos
{
    public class DoctorPatientDto
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; }
        public string PatientPhone { get; set; }
        public int PatientAge { get; set; }
        public string PatientId { get; set; }
        public Gender PatientGender { get; set; }

        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public AppointmentStatus Status { get; set; }

        public bool IsPaid { get; set; }
        public string? ClinicName { get; set; }
    }
}
