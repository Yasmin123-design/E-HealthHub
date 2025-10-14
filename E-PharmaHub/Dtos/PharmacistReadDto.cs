namespace E_PharmaHub.Dtos
{
    public class PharmacistReadDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string? PharmacistImage { get; set; }
        public string LicenseNumber { get; set; }
        public bool IsApproved { get; set; }
        public string PharmacyName { get; set; }
        public string PharmacyPhone { get; set; }
        public string? PharmacyImagePath { get; set; }
        public string? City { get; set; }
    }
}
