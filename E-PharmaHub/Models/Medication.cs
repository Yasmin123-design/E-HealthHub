using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{
    public class Medication
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Brand name is required.")]
        [StringLength(100, ErrorMessage = "Brand name cannot exceed 100 characters.")]
        public string BrandName { get; set; }

        [StringLength(100, ErrorMessage = "Generic name cannot exceed 100 characters.")]
        public string GenericName { get; set; }

        [StringLength(50, ErrorMessage = "Dosage form cannot exceed 50 characters.")]
        public string DosageForm { get; set; }

        [StringLength(50, ErrorMessage = "Strength cannot exceed 50 characters.")]
        public string Strength { get; set; }

        [StringLength(20, ErrorMessage = "ATC code cannot exceed 20 characters.")]
        public string ATCCode { get; set; }

        [StringLength(255, ErrorMessage = "Image path too long.")]
        public string? ImagePath { get; set; }


        public virtual ICollection<InventoryItem>? Inventories { get; set; }
        public virtual ICollection<AlternativeMedication>? Alternatives { get; set; }
    }
}
