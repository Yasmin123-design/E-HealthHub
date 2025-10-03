using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_PharmaHub.Models
{
    public class Appointment
    {
        [Key] public int Id { get; set; }

        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual AppUser User { get; set; }

        public string DoctorId { get; set; }
        [ForeignKey(nameof(DoctorId))]
        public virtual AppUser Doctor { get; set; }

        public int ClinicId { get; set; }
        public virtual Clinic Clinic { get; set; }

        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    }

}
