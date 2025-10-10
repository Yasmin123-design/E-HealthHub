using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories
{
    public interface IPaymentRepository
    {
        Task AddAsync(Payment payment);
        Task<Payment> GetByReferenceIdAsync(string referenceId);
        Task MarkAsSuccess(string sessionId);
        Task<Payment> GetByProviderTransactionIdAsync(string providerTransactionId);


    }
}
