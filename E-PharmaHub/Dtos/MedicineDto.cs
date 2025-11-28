using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Dtos
{
    public class MedicineDto
    {
        public int? Id { get; set; }
        public string? BrandName { get; set; }
        public string? GenericName { get; set; }
        public string? DosageForm { get; set; }
        public string? Strength { get; set; }
        public string? ATCCode { get; set; }
        public string? ImagePath { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public double? AverageRating { get; set; }

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
        public PharmacySimpleDto? Pharmacy { get; set; }
    }

}
