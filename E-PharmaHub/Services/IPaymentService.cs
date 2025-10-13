using E_PharmaHub.Models;

namespace E_PharmaHub.Services
{
    public interface IPaymentService
    {
        Task<object> VerifySessionAsync(string sessionId);
        Task<Payment> GetByReferenceIdAsync(string referenceId);
    }
}
