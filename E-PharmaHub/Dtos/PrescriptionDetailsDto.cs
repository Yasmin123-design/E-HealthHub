using E_PharmaHub.Models.Enums;

namespace E_PharmaHub.Dtos
{
    public class PrescriptionDetailsDto
    {
        public int Id { get; set; }
        public string DoctorName { get; set; }
        public Speciality DoctorSpecialty { get; set; }
        public string UserName { get; set; }
        public string Notes { get; set; }
        public DateTime IssuedAt { get; set; }
        public List<PrescriptionItemViewDto> Items { get; set; }
    }
}
