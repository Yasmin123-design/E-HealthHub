using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_PharmaHub.Models
{
    public class DonorProfile
    {
        public virtual AppUser AppUser { get; set; }
        public string AppUserId { get; set; }
        public BloodType BloodType { get; set; }
        public string City { get; set; }
        [Key] public int Id { get; set; }
        public bool IsAvailable { get; set; } = true;
        public DateTime? LastDonationDate { get; set; }
    }
}