using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories.PrescriptionRepo
{
    public interface IPrescriptionRepository
    {
        Task<Prescription> GetByIdAsync(int id);
        Task<List<Prescription>> GetByUserIdAsync(string userId);
        Task<Prescription?> GetWithItemsAsync(int id);
        Task AddAsync(Prescription prescription);
        Task DeleteAsync(Prescription prescription);
    }
}
