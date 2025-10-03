using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{
    public class Clinic
    {
        [Key] public int Id { get; set; }
        public string Name { get; set; }
        public int AddressId { get; set; }
        public string Phone { get; set; }
        public virtual Address Address { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}
