using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories
{
    public interface IPharmacistRepository : IGenericRepository<PharmacistProfile>
    {
        Task MarkAsPaid(string userId);

        Task<PharmacistProfile?> GetPharmacistByUserIdAsync(string userId);
        Task<bool> ApprovePharmacistAsync(int id);
        Task<bool> RejectPharmacistAsync(int id);
        Task<IEnumerable<PharmacistReadDto>> GetAllDetailsAsync();
        Task<PharmacistReadDto?> GetByIdDetailsAsync(int id);
        Task<PharmacistProfile?> GetByIdAsync(int id);

        Task<PharmacistProfile?> GetByUserIdAsync(string userId);


    }
}
