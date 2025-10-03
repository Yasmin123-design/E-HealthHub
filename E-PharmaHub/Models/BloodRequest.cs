using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{
    public class BloodRequest
    {
        [Key] public int Id { get; set; }
        public string RequestedByUserId { get; set; } 
        public BloodType RequiredType { get; set; }
        public string City { get; set; }
        public string HospitalName { get; set; }
        public int Units { get; set; }
        public bool Fulfilled { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual AppUser RequestedBy { get; set; }
        public virtual ICollection<DonorMatch> Matches { get; set; }
    }
}
