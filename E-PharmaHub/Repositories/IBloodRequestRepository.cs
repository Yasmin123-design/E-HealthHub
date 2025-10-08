using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories
{
    public interface IBloodRequestRepository : IGenericRepository<BloodRequest>
    {
        Task<IEnumerable<BloodRequest>> GetUnfulfilledRequestsAsync();
    }
}
