namespace E_PharmaHub.Dtos
{
    public class BriefOrderDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public decimal TotalPrice { get; set; }
        public int? PharmacyId { get; set; }
        public string Status { get; set; }
        public List<BriefOrderItemDto> Items { get; set; }
    }
}
