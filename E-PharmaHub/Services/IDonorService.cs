using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Services
{
    public interface IDonorService
    {
        Task<IEnumerable<DonorProfile>> GetAllAsync();
        Task<IEnumerable<DonorProfile>> GetByFilterAsync(BloodType? type, string? city);
        Task<DonorProfile?> GetByUserIdAsync(string userId);
        Task<DonorProfile> RegisterAsync(DonorRegisterDto donor);
        Task<bool> UpdateAvailabilityAsync(string userId, bool isAvailable);
        Task UpdateAsync(DonorProfile donor);
        Task DeleteAsync(int id);
    }
}
