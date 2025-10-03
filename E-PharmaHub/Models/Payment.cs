using System.ComponentModel.DataAnnotations;

namespace E_PharmaHub.Models
{
    public class Payment
    {
        [Key] public int Id { get; set; }
        public int OrderId { get; set; }
        public string ProviderTransactionId { get; set; } 
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public decimal Amount { get; set; }
        public DateTime ProcessedAt { get; set; }

        public virtual Order Order { get; set; }
    }
}
