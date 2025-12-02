using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.Models.Enums;

namespace E_PharmaHub.Repositories
{
    public interface IDonorRepository : IGenericRepository<DonorProfile>
    {
        Task<IEnumerable<DonorReadDto>> GetByFilterAsync(BloodType? type, string? city);
        Task<DonorProfile?> GetByUserIdAsync(string userId);
        Task<bool> UpdateAvailabilityAsync(string userId, bool isAvailable);
        Task<IEnumerable<DonorReadDto>> GetAllDetailsAsync();
    }
}
