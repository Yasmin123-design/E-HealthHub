using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }
        public string PaymentIntentId { get; set; }
        public string ReferenceId { get; set; }
        public PaymentForType PaymentFor { get; set; }
        public string ProviderTransactionId { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public decimal Amount { get; set; }
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;

        public string PayerUserId { get; set; }
    }

}
