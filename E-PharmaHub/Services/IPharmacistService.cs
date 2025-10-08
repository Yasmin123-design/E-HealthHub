using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Services
{
    public interface IPharmacistService
    {
        Task<AppUser> RegisterPharmacistAsync(PharmacistRegisterDto dto, IFormFile image);

        Task AddPharmacistAsync(PharmacistProfile pharmacist);

        Task<IEnumerable<PharmacistProfile>> GetAllPharmacistsAsync();

        Task<PharmacistProfile?> GetPharmacistByIdAsync(int id);

        Task UpdatePharmacistAsync(int id, PharmacistProfile updatedPharmacist,IFormFile? newImage);

        Task<PharmacistProfile?> GetPharmacistByUserIdAsync(string userId);

        Task DeletePharmacistAsync(int id);
        Task<bool> ApprovePharmacistAsync(int id);
        Task<bool> RejectPharmacistAsync(int id);

    }


}
