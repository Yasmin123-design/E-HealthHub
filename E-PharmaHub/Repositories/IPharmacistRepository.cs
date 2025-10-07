using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories
{
    public interface IPharmacistRepository : IGenericRepository<PharmacistProfile>
    {
        Task<PharmacistProfile?> GetPharmacistByUserIdAsync(string userId);
    }
}
