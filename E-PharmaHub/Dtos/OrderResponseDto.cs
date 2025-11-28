namespace E_PharmaHub.Dtos
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string? UserImage { get; set; }
        public string UserEmail { get; set; }
        public int PharmacyId { get; set; }
        public string PharmacyName { get; set; }
        public string? PharmacyImage { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Street { get; set; }
        public string? PhoneNumber { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderResponseItemDto>? Items { get; set; }
    }
}
