using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_PharmaHub.Models
{
    public class DoctorProfile
    {
        [Key] public int Id { get; set; }
        public string Specialty { get; set; }
        public int ClinicId { get; set; }
        public bool IsApproved { get; set; } = false;
        public bool IsRejected { get; set; } = false;
        public bool HasPaid { get; set; } = false;

        public virtual Clinic? Clinic { get; set; }
        public string? AppUserId { get; set; }
        public virtual AppUser? AppUser { get; set; }
    }
}
