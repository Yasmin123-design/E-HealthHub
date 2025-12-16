using E_PharmaHub.Models;
using E_PharmaHub.Models.Enums;

namespace E_PharmaHub.Dtos
{
    public class AppointmentResponseDto
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public decimal? AppointmentAmount { get; set; }
        public string DoctorName { get; set; }
        public Speciality DoctorSpeciality { get; set; }
        public string? DoctorImage { get; set; }
        public string UserId { get; set; }
        public string UserNameLogged { get; set; }
        public string? UserImageLogged { get; set; }
        public int ClinicId { get; set; }
        public string ClinicName { get; set; }
        public string? ClinicImage { get; set; }
        public string PatientName { get; set; }
        public string PatientPhone { get; set; }
        public int PatientAge { get; set; }
        public Gender PatientGender { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }

        public AppointmentStatus Status { get; set; }
    }
}
