using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_PharmaHub.Models
{
    public class PharmacistProfile
    {
        [Key] public int Id { get; set; }
        public int PharmacyId { get; set; } 
        public string AppUserId { get; set; }
        public string LicenseNumber { get; set; }

        public virtual AppUser AppUser { get; set; }
        public virtual Pharmacy Pharmacy { get; set; }
    }
}
