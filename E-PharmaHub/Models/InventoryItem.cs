using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{
    public class InventoryItem
    {
        [Key] public int Id { get; set; }
        public int PharmacyId { get; set; }
        public int MedicationId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime LastUpdated { get; set; }

        public virtual Pharmacy Pharmacy { get; set; }
        public virtual Medication Medication { get; set; }
    }
}
