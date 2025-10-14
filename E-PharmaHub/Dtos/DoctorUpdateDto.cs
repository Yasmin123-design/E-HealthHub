namespace E_PharmaHub.Dtos
{
    public class DoctorUpdateDto
    {
        public string? Specialty { get; set; }

        public string? Email { get; set; }
        public string? UserName { get; set; }

        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }

    }
}
