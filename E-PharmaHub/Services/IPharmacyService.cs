using E_PharmaHub.Models;

namespace E_PharmaHub.Services
{
    public interface IPharmacyService
    {
        Task<IEnumerable<Pharmacy>> GetAllPharmaciesAsync();
        Task<Pharmacy> GetPharmacyByIdAsync(int id);
        Task AddPharmacyAsync(Pharmacy pharmacy , IFormFile imageFile);
        Task UpdatePharmacyAsync(int id , Pharmacy pharmacy , IFormFile imageFile);
        Task DeletePharmacyAsync(int id);
    }
}
