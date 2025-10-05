using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{
    public class Address
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Country must be between 2 and 100 characters.")]
        public string Country { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "City must be between 2 and 100 characters.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Street is required.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Street must be between 3 and 150 characters.")]
        public string Street { get; set; }

        [StringLength(20, ErrorMessage = "PostalCode can't exceed 20 characters.")]
        public string PostalCode { get; set; }

        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
        public double? Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]
        public double? Longitude { get; set; }
    }


}
