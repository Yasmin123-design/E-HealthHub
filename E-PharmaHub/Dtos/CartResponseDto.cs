namespace E_PharmaHub.Dtos
{
    public class CartResponseDto
    {
        public int CartId { get; set; }
        public List<CartPharmacyGroupDto> Pharmacies { get; set; }
    }
}
