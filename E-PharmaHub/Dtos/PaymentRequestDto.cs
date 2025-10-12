using E_PharmaHub.Models;

namespace E_PharmaHub.Dtos
{
    public class PaymentRequestDto
    {
        public string? ReferenceId { get; set; }
        public PaymentForType PaymentFor { get; set; }
        public decimal Amount { get; set; }
    }
}
