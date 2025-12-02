using E_PharmaHub.Models.Enums;

namespace E_PharmaHub.Dtos
{
    public class DoctorUpdateDto
    {
        public string? Specialty { get; set; }
        public Gender Gender { get; set; }
        public decimal ConsultationPrice { get; set; }
        public ConsultationType ConsultationType { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }

        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }

    }
}
