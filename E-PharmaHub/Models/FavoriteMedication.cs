namespace E_PharmaHub.Models
{
    public class FavoriteMedication
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int MedicationId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual AppUser User { get; set; }
        public virtual Medication Medication { get; set; }
    }
}
