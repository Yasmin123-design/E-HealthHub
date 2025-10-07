using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace E_PharmaHub.Repositories
{
    public class InventoryItemRepository : IInventoryItemRepository
    {
        private readonly EHealthDbContext _context;

        public InventoryItemRepository(EHealthDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InventoryItem>> GetAllAsync()
        {
            return await _context.InventoryItems
                .Include(i => i.Medication)
                .Include(i => i.Pharmacy)
                .ToListAsync();
        }

        public async Task<InventoryItem> GetByIdAsync(int id)
        {
            return await _context.InventoryItems
                .Include(i => i.Medication)
                .Include(i => i.Pharmacy)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task AddAsync(InventoryItem entity)
        {
            await _context.InventoryItems.AddAsync(entity);
        }
        public async Task<InventoryItem?> FindAsync(Expression<Func<InventoryItem, bool>> predicate)
        {
            return await _context.InventoryItems
                                 .Include(i => i.Medication)
                                 .Include(i => i.Pharmacy)
                                 .FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<InventoryItem>> FindAllAsync(Expression<Func<InventoryItem, bool>> predicate)
        {
            return await _context.InventoryItems
                                 .Include(i => i.Medication)
                                 .Include(i => i.Pharmacy)
                                 .Where(predicate)
                                 .ToListAsync();
        }

        public void Update(InventoryItem entity)
        {
            _context.InventoryItems.Update(entity);
        }

        public void Delete(InventoryItem entity)
        {
            _context.InventoryItems.Remove(entity);
        }

        public async Task<IEnumerable<InventoryItem>> GetByPharmacyIdAsync(int pharmacyId)
        {
            return await _context.InventoryItems
                .Include(i => i.Medication)
                .Where(i => i.PharmacyId == pharmacyId)
                .ToListAsync();
        }

        public async Task<IEnumerable<InventoryItem>> GetByMedicationIdAsync(int medicationId)
        {
            return await _context.InventoryItems
                .Include(i => i.Pharmacy)
                .Where(i => i.MedicationId == medicationId)
                .ToListAsync();
        }
    }

}
