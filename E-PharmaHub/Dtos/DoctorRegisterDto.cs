using E_PharmaHub.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Dtos
{
    public class DoctorRegisterDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "UserName is required.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Specialty is required.")]
        [StringLength(100, ErrorMessage = "Specialty cannot exceed 100 characters.")]
        public string Specialty { get; set; }

        public Gender Gender { get; set; }
        public decimal? ConsultationPrice { get; set; }
        public ConsultationType ConsultationType { get; set; }


        [Required(ErrorMessage = "Clinic name is required.")]
        [StringLength(150, ErrorMessage = "Clinic name cannot exceed 150 characters.")]
        public string ClinicName { get; set; }

        [Required(ErrorMessage = "Clinic phone is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string ClinicPhone { get; set; }

        [Required(ErrorMessage = "Clinic address is required.")]
        public AddressDto ClinicAddress { get; set; }

    }


}
