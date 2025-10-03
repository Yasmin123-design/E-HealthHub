using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{
    public class DonorMatch
    {
        [Key] public int Id { get; set; }
        public int BloodRequestId { get; set; }
        public string DonorUserId { get; set; }
        public DateTime MatchedAt { get; set; } = DateTime.UtcNow;
        public bool Notified { get; set; }

        public virtual BloodRequest BloodRequest { get; set; }
        public virtual AppUser DonorUser { get; set; }
    }
}
