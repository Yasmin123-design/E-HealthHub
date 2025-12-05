using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Dtos
{
    public class PharmacyUpdateDto
    {
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Pharmacy name must be between 3 and 100 characters.")]
        public string? Name { get; set; }

        public int? AddressId { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format.")]
        [StringLength(15, ErrorMessage = "Phone number can't exceed 15 digits.")]
        public string? Phone { get; set; }
        public decimal? DeliveryFee { get; set; }

    }

}
