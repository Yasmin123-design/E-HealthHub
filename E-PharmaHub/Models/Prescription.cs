using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{
    public class Prescription
    {
        [Key] public int Id { get; set; }
        public string UserId { get; set; }
        public int DoctorId { get; set; }
        public string Notes { get; set; }
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
        public virtual DoctorProfile Doctor { get; set; }

        public virtual ICollection<PrescriptionItem>? Items { get; set; }
        public virtual AppUser? User { get; set; }
    }
}
