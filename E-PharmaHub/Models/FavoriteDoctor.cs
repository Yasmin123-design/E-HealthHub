namespace E_PharmaHub.Models
{
    public class FavoriteDoctor
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int DoctorId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual AppUser User { get; set; }
        public virtual DoctorProfile Doctor { get; set; }
    }
}
