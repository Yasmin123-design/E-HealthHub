using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories
{
    public interface IInventoryItemRepository : IGenericRepository<InventoryItem>
    {
        Task<IEnumerable<InventoryItem>> GetByPharmacyIdAsync(int pharmacyId);
        Task<IEnumerable<InventoryItem>> GetByMedicationIdAsync(int medicationId);
    }

}
