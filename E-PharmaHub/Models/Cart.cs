using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual AppUser? User { get; set; }
        public virtual ICollection<CartItem>? Items { get; set; }
    }
}

