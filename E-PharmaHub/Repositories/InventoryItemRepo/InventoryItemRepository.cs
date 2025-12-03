using E_PharmaHub.Dtos;
using E_PharmaHub.Helpers;
using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace E_PharmaHub.Repositories.InventoryItemRepo
{
    public class InventoryItemRepository : IInventoryItemRepository
    {
        private readonly EHealthDbContext _context;

        public InventoryItemRepository(EHealthDbContext context)
        {
            _context = context;
        }

        private IQueryable<InventoryItem> BaseInventoryIncludes()
        {
            return _context.InventoryItems
                .AsNoTracking()
                .Include(i => i.Medication)
                    .ThenInclude(m => m.Reviews)
                .Include(i => i.Pharmacy)
                    .ThenInclude(p => p.Address);
        }

        public async Task<IEnumerable<InventoryItem>> GetAllAsync()
        {
            return await BaseInventoryIncludes().ToListAsync();
        }

        public async Task<IEnumerable<MedicineDto>> GetAlternativeMedicinesAsync(int medicineId)
        {
            var originalMedicine = await _context.Medications.FindAsync(medicineId);
            if (originalMedicine == null)
                return Enumerable.Empty<MedicineDto>();

            var alternatives = await BaseInventoryIncludes()
                .Where(i =>
                    i.Medication.Id != medicineId &&
                    (i.Medication.GenericName == originalMedicine.GenericName ||
                     i.Medication.ATCCode == originalMedicine.ATCCode))
                .ToListAsync();

            return alternatives
                .Select(MedicineSelector.MapInventoryToDto)
                .ToList();
        }


        public async Task<MedicineDto?> GetByIdAsync(int id)
        {
            var inventoryItem = await BaseInventoryIncludes()
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventoryItem == null) return null;

            return MedicineSelector.MapInventoryToDto(inventoryItem);
        }
        public async Task<InventoryItem?> GetInventoryItemByIdAsync(int id)
        {
            var inventoryItem = await BaseInventoryIncludes()
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventoryItem == null) return null;

            return inventoryItem;
        }
        public async Task AddAsync(InventoryItem entity)
        {
            await _context.InventoryItems.AddAsync(entity);
        }

        public async Task<InventoryItem?> FindAsync(Expression<Func<InventoryItem, bool>> predicate)
        {
            return await BaseInventoryIncludes()
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<InventoryItem>> FindAllAsync(Expression<Func<InventoryItem, bool>> predicate)
        {
            return await BaseInventoryIncludes()
                .Where(predicate)
                .ToListAsync();
        }

        public async Task Update(InventoryItem entity)
        {
            _context.InventoryItems.Update(entity);
        }

        public void Delete(InventoryItem entity)
        {
            _context.InventoryItems.Remove(entity);
        }

        public async Task<IEnumerable<MedicineDto>> GetByPharmacyIdAsync(int pharmacyId)
        {
            var items = await BaseInventoryIncludes()
                .Where(i => i.PharmacyId == pharmacyId)
                .ToListAsync();

            return items.Select(MedicineSelector.MapInventoryToDto).ToList();
        }

        public async Task<IEnumerable<MedicineDto>> GetByMedicationIdAsync(int medicationId)
        {
            var items = await BaseInventoryIncludes()
                .Where(i => i.MedicationId == medicationId)
                .ToListAsync();

            return items.Select(MedicineSelector.MapInventoryToDto).ToList();
        }

        public async Task<InventoryItem?> GetByPharmacyAndMedicationAsync(int pharmacyId, int medicationId)
        {
            var item = await BaseInventoryIncludes()
                .FirstOrDefaultAsync(i => i.PharmacyId == pharmacyId && i.MedicationId == medicationId);

            if (item == null) return null;

            return item;
        }

        public async Task<InventoryItem> GetInventoryForCheckoutAsync(int medicationId, int pharmacyId, decimal price)
        {
            return await _context.InventoryItems
                .AsNoTracking()
                .FirstOrDefaultAsync(i =>
                    i.MedicationId == medicationId &&
                    i.PharmacyId == pharmacyId &&
                    i.Price == price);
        }

        public async Task DecreaseQuantityAsync(int inventoryId, int quantity)
        {
            var inventory = await _context.InventoryItems.FindAsync(inventoryId);
            if (inventory != null)
            {
                inventory.Quantity -= quantity;
                _context.InventoryItems.Update(inventory);
            }
        }
    }
}
