using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{
    public class Review
    {
        [Key] public int Id { get; set; }
        public string UserId { get; set; }
        public int? PharmacyId { get; set; }
        public int? MedicationId { get; set; }
        public int Rating { get; set; } 
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual AppUser User { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }
        public virtual Medication Medication { get; set; }
    }
}
