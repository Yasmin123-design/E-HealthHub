using E_PharmaHub.Models;

namespace E_PharmaHub.Services
{
    public interface IBloodRequestService
    {
        Task<IEnumerable<BloodRequest>> GetAllRequestsAsync();
        Task<BloodRequest?> GetRequestByIdAsync(int id);
        Task<IEnumerable<BloodRequest>> GetUnfulfilledRequestsAsync();
        Task<BloodRequest> AddRequestAsync(BloodRequest request);
        Task<bool> UpdateRequestAsync(int id, BloodRequest updatedRequest);
        Task<bool> DeleteRequestAsync(int id);
    }

}
