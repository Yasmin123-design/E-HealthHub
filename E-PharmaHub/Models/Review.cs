using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        public string? UserId { get; set; }

        public int? PharmacyId { get; set; }
        public int? DoctorId { get; set; }
        public int? MedicationId { get; set; }

        [Required(ErrorMessage = "Rating is required.")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [StringLength(500, ErrorMessage = "Comment can't exceed 500 characters.")]
        public string Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual AppUser? User { get; set; }
        public virtual Pharmacy? Pharmacy { get; set; }
        public virtual DoctorProfile? Doctor { get; set; }
        public virtual Medication? Medication { get; set; }
    }
}
