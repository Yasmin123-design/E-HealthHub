namespace E_PharmaHub.Dtos
{
    public class CartResponseDto
    {
        public int CartId { get; set; }
        public List<CartPharmacyGroupDto> Pharmacies { get; set; }
        public decimal OrderTotal => Pharmacies.Sum(p => p.TotalPrice);
        public decimal DeliveryTotal => Pharmacies.Sum(p => p.DeliveryFee);
        public decimal GrandTotal => OrderTotal + DeliveryTotal;
    }
}
