using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{
    public class PharmacistProfile
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "PharmacyId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Pharmacy ID.")]
        public int PharmacyId { get; set; }

        public string? AppUserId { get; set; }

        [Required(ErrorMessage = "License number is required.")]
        [StringLength(50, ErrorMessage = "License number cannot exceed 50 characters.")]
        public string LicenseNumber { get; set; }

        public virtual AppUser? AppUser { get; set; }

        public virtual Pharmacy? Pharmacy { get; set; }
    }
}
