using E_PharmaHub.Models.Enums;

namespace E_PharmaHub.Dtos
{
    public class DoctorUpdateDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public Speciality? Specialty { get; set; }
        public Gender? Gender { get; set; }

    }
}
