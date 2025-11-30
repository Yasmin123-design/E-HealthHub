namespace E_PharmaHub.Dtos
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public string Medication { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total => Quantity * UnitPrice;
    }
}
