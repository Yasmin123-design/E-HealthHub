using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Dtos
{
    public class PharmacistRegisterDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Pharmacy name is required.")]
        [StringLength(150, ErrorMessage = "Pharmacy name cannot exceed 150 characters.")]
        public string PharmacyName { get; set; }

        [Required(ErrorMessage = "License number is required.")]
        [StringLength(50, ErrorMessage = "License number cannot exceed 50 characters.")]
        public string LicenseNumber { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; }


        [Required(ErrorMessage = "Address is required.")]
        public AddressDto Address { get; set; }
    }


}
