namespace E_PharmaHub.Dtos
{
    public class DonorReadDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string BloodType { get; set; }
        public string City { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime? LastDonationDate { get; set; }
    }
}
