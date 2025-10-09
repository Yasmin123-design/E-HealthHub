namespace E_PharmaHub.Dtos
{
    public class DoctorReadDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Specialty { get; set; }
        public bool IsApproved { get; set; }

        public string ClinicName { get; set; }
        public string ClinicPhone { get; set; }
        public string ClinicImagePath { get; set; }
        public string City { get; set; }
    }

}
