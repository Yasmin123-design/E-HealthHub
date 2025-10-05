using E_PharmaHub.Models;

namespace E_PharmaHub.Services
{
    public interface IInventoryService
    {
        Task AddInventoryItemAsync(InventoryItem item);
        Task UpdateInventoryItemAsync(InventoryItem item);
        Task DeleteInventoryItemAsync(int id);
        Task<InventoryItem> GetInventoryItemByIdAsync(int id);
        Task<IEnumerable<InventoryItem>> GetInventoryByPharmacyAsync(int pharmacyId);
    }

}
