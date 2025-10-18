namespace E_PharmaHub.Dtos
{
    public class PrescriptionItemDto
    {
        public int? MedicationId { get; set; }      
        public string? MedicationName { get; set; } 
        public string Dosage { get; set; }
        public int Quantity { get; set; }
    }

}
