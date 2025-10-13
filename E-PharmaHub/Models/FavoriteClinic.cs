namespace E_PharmaHub.Models
{
    public class FavoriteClinic
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ClinicId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual AppUser User { get; set; }
        public virtual Clinic Clinic { get; set; }
    }
}
