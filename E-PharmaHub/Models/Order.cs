using E_PharmaHub.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }
        public int PharmacyId { get; set; }
        public int? PaymentId { get; set; }

        [Required, MaxLength(100)]
        public string Country { get; set; }

        [Required, MaxLength(100)]
        public string City { get; set; }

        [Required, MaxLength(250)]
        public string Street { get; set; }

        [Required, MaxLength(20)]
        public string PhoneNumber { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual AppUser? User { get; set; }
        public virtual Pharmacy? Pharmacy { get; set; }
        public virtual ICollection<OrderItem>? Items { get; set; }
        public virtual Payment? Payment { get; set; }
    }

}
