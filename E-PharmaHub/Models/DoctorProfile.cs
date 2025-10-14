using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{
    public class DoctorProfile
    {
        [Key] public int Id { get; set; }
        [Required(ErrorMessage = "Specialty is required.")]
        [StringLength(100, ErrorMessage = "Specialty cannot exceed 100 characters.")]
        public string Specialty { get; set; }
        public string? Image { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "ClinicId must be a valid positive number.")]
        public int? ClinicId { get; set; }
        public bool IsApproved { get; set; } = false;
        public bool IsRejected { get; set; } = false;
        public bool HasPaid { get; set; } = false;

        public virtual Clinic? Clinic { get; set; }
        public string? AppUserId { get; set; }
        public virtual AppUser? AppUser { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }

    }
}
