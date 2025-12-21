using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{
    public class DoctorAvailability
    {
        [Key]
        public int Id { get; set; }

        public int DoctorProfileId { get; set; }
        public virtual DoctorProfile? DoctorProfile { get; set; }

        public DayOfWeek DayOfWeek { get; set; }   

        public TimeSpan StartTime { get; set; }   
        public TimeSpan EndTime { get; set; }     

        public int SlotDurationInMinutes { get; set; } = 30;
    }
}
