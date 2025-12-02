using E_PharmaHub.Models;
using E_PharmaHub.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;

namespace E_PharmaHub.Repositories.PaymentRepo
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly EHealthDbContext _context;

        public PaymentRepository(EHealthDbContext context)
        {
            _context = context;
        }
        public void Delete(Payment entity)
        {
            _context.Payments.Remove(entity);
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

        public async Task<Payment> GetByIdAsync(int id)
        {
            return await _context.Payments.FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
