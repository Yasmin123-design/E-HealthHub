using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories.PaymentRepo
{
    public interface IPaymentRepository
    {
        Task<Payment> GetByCheckoutSessionIdAsync(string sessionId);
        Task<Payment> GetByPaymentIntendIdAsync(string paymentIntentId);
        void Delete(Payment entity);
        Task AddAsync(Payment payment);
        Task<Payment> GetByReferenceIdAsync(string referenceId);
        Task<Payment> GetByIdAsync(int id);
        Task MarkAsSuccess(string sessionId);
        Task<Payment> GetByProviderTransactionIdAsync(string providerTransactionId);


    }
}
