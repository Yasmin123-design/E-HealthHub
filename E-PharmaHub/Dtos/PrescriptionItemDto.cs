namespace E_PharmaHub.Dtos
{
    public class PrescriptionItemDto
    {
        public int? Id { get; set; } 
        public string MedicationName { get; set; }
        public string MedicationStrength { get; set; }
        public string Dosage { get; set; }
        public int Quantity { get; set; }
        public string Duration { get; set; }
        public string Notes { get; set; }
    }

}
