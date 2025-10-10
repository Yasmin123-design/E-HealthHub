using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{
    public class PharmacistProfile
    {
        [Key]
        public int Id { get; set; }
        public int PharmacyId { get; set; }
        public bool IsApproved { get; set; } = false;
        public bool IsRejected { get; set; } = false;
        public bool HasPaid { get; set; } = false;

        public string? AppUserId { get; set; }

        [Required(ErrorMessage = "License number is required.")]
        [StringLength(50, ErrorMessage = "License number cannot exceed 50 characters.")]
        public string LicenseNumber { get; set; }

        public virtual AppUser? AppUser { get; set; }

        public virtual Pharmacy? Pharmacy { get; set; }
    }
}
