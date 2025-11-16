using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories
{
    public interface IPaymentRepository
    {
        void Delete(Payment entity);
        Task AddAsync(Payment payment);
        Task<Payment> GetByReferenceIdAsync(string referenceId);
        Task<Payment> GetByIdAsync(int id);
        Task MarkAsSuccess(string sessionId);
        Task<Payment> GetByProviderTransactionIdAsync(string providerTransactionId);


    }
}
