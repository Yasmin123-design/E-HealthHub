using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_PharmaHub.Models
{
    public class HealthProviderProfile
    {
        [Key] public int Id { get; set; }
        public string OrganizationName { get; set; }
        public string ServicesOffered { get; set; } 
        public virtual AppUser AppUser { get; set; }
        public string AppUserId { get; set; }
    }
}
