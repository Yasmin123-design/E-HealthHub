namespace E_PharmaHub.Dtos
{
    public class OrderResponseItemDto
    {
        public int MedicationId { get; set; }
        public string MedicicationImage { get; set; }
        public string MedicationName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
