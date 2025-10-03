using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{
    public class Medication
    {
        [Key] public int Id { get; set; }
        [Required] public string BrandName { get; set; }
        public string GenericName { get; set; }
        public string DosageForm { get; set; }
        public string Strength { get; set; }
        public string ATCCode { get; set; }
        public virtual ICollection<InventoryItem> Inventories { get; set; }
        public virtual ICollection<AlternativeMedication> Alternatives { get; set; }
    }
}
