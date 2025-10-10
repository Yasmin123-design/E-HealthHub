using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace E_PharmaHub.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly EHealthDbContext _context;

        public PaymentRepository(EHealthDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> GetByReferenceIdAsync(string referenceId)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p => p.ReferenceId == referenceId);
        }
        public async Task MarkAsSuccess(string sessionId)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.ProviderTransactionId == sessionId);

            if (payment == null)
                throw new Exception("Payment not found for this session.");

            payment.Status = PaymentStatus.Paid;
            payment.ProcessedAt = DateTime.UtcNow;

        }
        public async Task<Payment> GetByProviderTransactionIdAsync(string providerTransactionId)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p => p.ProviderTransactionId == providerTransactionId);
        }
        public async Task AddAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
        }
    }
}
