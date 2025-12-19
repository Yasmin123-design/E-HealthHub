using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories.PrescriptionItemRepo
{
    public interface IPrescriptionItemRepository
    {
        Task<PrescriptionItem?> GetByIdAsync(int id);
        Task<List<PrescriptionItem>> GetByPrescriptionIdAsync(int prescriptionId);
        Task AddAsync(PrescriptionItem item);
        void Update(PrescriptionItem item);
        void Delete(PrescriptionItem item);
    }
}
