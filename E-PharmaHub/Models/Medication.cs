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

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [StringLength(300, ErrorMessage = "Warning cannot exceed 300 characters")]
        public string? Warning { get; set; }

        [StringLength(200, ErrorMessage = "SuitableFor cannot exceed 200 characters")]
        public string? SuitableFor { get; set; }

        [StringLength(200, ErrorMessage = "NotSuitableFor cannot exceed 200 characters")]
        public string? NotSuitableFor { get; set; }

        [StringLength(500, ErrorMessage = "Composition cannot exceed 500 characters")]
        public string? Composition { get; set; }

        [StringLength(500, ErrorMessage = "DirectionsForUse cannot exceed 500 characters")]
        public string? DirectionsForUse { get; set; }


        [StringLength(255, ErrorMessage = "Image path too long.")]
        public string? ImagePath { get; set; }


        public virtual ICollection<InventoryItem>? Inventories { get; set; }
        public virtual ICollection<AlternativeMedication>? Alternatives { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }

    }
}
