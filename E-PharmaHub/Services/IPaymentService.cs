namespace E_PharmaHub.Services
{
    public interface IPaymentService
    {
        Task<object> VerifySessionAsync(string sessionId);
    }
}
