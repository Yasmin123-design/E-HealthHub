using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{
    public class Notification
    {
        [Key] public int Id { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public virtual AppUser? User { get; set; }
    }
}
