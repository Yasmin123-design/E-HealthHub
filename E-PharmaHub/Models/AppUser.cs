using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{

    public class AppUser : IdentityUser
    {
        public UserRole Role { get; set; } = UserRole.RegularUser;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsApproved { get; set; } = false; 
        public virtual DonorProfile? DonorProfile { get; set; }
        public virtual PharmacistProfile? PharmacistProfile { get; set; }
        public virtual DoctorProfile? DoctorProfile { get; set; }
        public virtual HealthProviderProfile? HealthProviderProfile { get; set; }

        public virtual ICollection<Order>? Orders { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }
        public virtual ICollection<MessageThread>? MessageThreads { get; set; }
        public virtual ICollection<Appointment>? PatientAppointments { get; set; }
        public virtual ICollection<Appointment>? DoctorAppointments { get; set; }
    }

}
