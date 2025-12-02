using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Services.PharmacistServ
{
    public interface IPharmacistService
    {
        Task MarkAsPaid(string userId);
        Task<AppUser> RegisterPharmacistAsync(PharmacistRegisterDto dto, IFormFile pharmacyImage, IFormFile pharmacistImage);

        Task AddPharmacistAsync(PharmacistProfile pharmacist);
        Task<PharmacistProfile?> GetPharmacistProfileByIdAsync(int id);
        Task<IEnumerable<PharmacistReadDto>> GetAllPharmacistsAsync();
        Task<PharmacistReadDto?> GetPharmacistByIdAsync(int id);
        Task<PharmacistReadDto?> GetPharmacistByUserIdAsync(string userId);

        Task<PharmacistProfile?> GetPharmacistProfileByUserIdAsync(string userId);

        Task<bool> UpdatePharmacistProfileAsync(string userId, PharmacistUpdateDto dto, IFormFile? image);

        Task DeletePharmacistAsync(int id);
        Task<(bool success, string message)> RejectPharmacistAsync(int pharmacistId);
        Task<(bool success, string message)> ApprovePharmacistAsync(int pharmacistId);

    }


}
