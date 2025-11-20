using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{

    public class AppUser : IdentityUser
    {
        public UserRole Role { get; set; } = UserRole.RegularUser;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? ProfileImage { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }
        public virtual DonorProfile? DonorProfile { get; set; }
        public virtual PharmacistProfile? PharmacistProfile { get; set; }
        public virtual DoctorProfile? DoctorProfile { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }
        public virtual ICollection<MessageThread>? MessageThreads { get; set; }
        public virtual ICollection<Appointment>? PatientAppointments { get; set; }
        public virtual ICollection<Appointment>? DoctorAppointments { get; set; }
    }

}
