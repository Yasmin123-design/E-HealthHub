using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Dtos
{
    public class MedicineDto
    {
        public int Id { get; set; }
        public string BrandName { get; set; }
        public string GenericName { get; set; }
        public string DosageForm { get; set; }
        public string Strength { get; set; }
        public string ATCCode { get; set; }
        public string? ImagePath { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public double AverageRating { get; set; }
        public PharmacySimpleDto? Pharmacy { get; set; }
    }

}
