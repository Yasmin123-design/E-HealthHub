using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{
    public class PrescriptionItem
    {
        [Key] public int Id { get; set; }
        public int PrescriptionId { get; set; }
        public int MedicationId { get; set; }
        public string Dosage { get; set; }
        public int Quantity { get; set; }

        public virtual Prescription? Prescription { get; set; }
        public virtual Medication? Medication { get; set; }
    }
}
