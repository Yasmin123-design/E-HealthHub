namespace E_PharmaHub.Dtos
{
    public class CartPharmacyGroupDto
    {
        public int PharmacyId { get; set; }
        public string PharmacyName { get; set; }
        public List<CartItemDto> Items { get; set; }
        public decimal TotalPrice => Items.Sum(i => i.Total);
    }
}
