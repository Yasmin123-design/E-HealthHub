namespace E_PharmaHub.Dtos
{
    public class PharmacistUpdateDto
    {
        public string? LicenseNumber { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }

        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }
    }
}
