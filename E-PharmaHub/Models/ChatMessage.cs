using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{
    public class ChatMessage
    {
        [Key] public int Id { get; set; }
        public int ThreadId { get; set; }
        public string SenderId { get; set; }
        public string Text { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool Read { get; set; } = false;

        public virtual MessageThread? Thread { get; set; }
        public virtual AppUser? Sender { get; set; }
    }
}
