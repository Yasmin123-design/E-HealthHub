using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{
    public class MessageThread
    {
        [Key] public int Id { get; set; }
        public string Title { get; set; } 
        public virtual ICollection<MessageThreadParticipant> Participants { get; set; }
        public virtual ICollection<ChatMessage> Messages { get; set; }
    }
}
