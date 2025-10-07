using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Dtos
{
    public class MedicineDto
    {
        [Required]
        [StringLength(100)]
        public string BrandName { get; set; }

        [StringLength(100)]
        public string? GenericName { get; set; }

        [StringLength(50)]
        public string? DosageForm { get; set; }

        [StringLength(50)]
        public string? Strength { get; set; }

        [StringLength(20)]
        public string? ATCCode { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
    }

}
