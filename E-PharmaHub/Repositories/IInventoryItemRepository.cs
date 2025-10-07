using E_PharmaHub.Models;
using System.Linq.Expressions;

namespace E_PharmaHub.Repositories
{
    public interface IInventoryItemRepository : IGenericRepository<InventoryItem>
    {
        Task<IEnumerable<InventoryItem>> GetByPharmacyIdAsync(int pharmacyId);
        Task<IEnumerable<InventoryItem>> GetByMedicationIdAsync(int medicationId);
        Task<InventoryItem?> FindAsync(Expression<Func<InventoryItem, bool>> predicate);
        Task<IEnumerable<InventoryItem>> FindAllAsync(Expression<Func<InventoryItem, bool>> predicate);
    }

}
