using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{
    public class MessageThreadParticipant
    {
        [Key] public int Id { get; set; }
        public int ThreadId { get; set; }
        public string UserId { get; set; }
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
        public virtual MessageThread? Thread { get; set; }
        public virtual AppUser? User { get; set; }
    }
}
