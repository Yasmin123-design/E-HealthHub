using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{
    public class PrescriptionItem
    {
        [Key]
        public int Id { get; set; }

        public int? PrescriptionId { get; set; }
        public string? MedicationName { get; set; } 
        public string? MedicationStrength { get; set; }
        public string? Duration { get; set; }
        public string? Notes { get; set; }
        public string Dosage { get; set; }
        public int Quantity { get; set; }

        public virtual Prescription? Prescription { get; set; }
    }

}
