using E_PharmaHub.Dtos;
using E_PharmaHub.Helpers;
using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace E_PharmaHub.Repositories.InventoryItemRepo
{
    public interface IInventoryItemRepository
    {
        Task<InventoryItem> GetInventoryForCheckoutAsync(int medicationId, int pharmacyId, decimal price);
        Task DecreaseQuantityAsync(int inventoryId, int quantity);
        Task<IEnumerable<InventoryItem>> GetAllAsync();
        Task<IEnumerable<MedicineDto>> GetAlternativeMedicinesAsync(string name);
        Task<MedicineDto?> GetByIdAsync(int id);
        Task AddAsync(InventoryItem entity);

        Task<InventoryItem?> FindAsync(Expression<Func<InventoryItem, bool>> predicate);
        Task<IEnumerable<InventoryItem>> FindAllAsync(Expression<Func<InventoryItem, bool>> predicate);
        Task Update(InventoryItem entity);
        void Delete(InventoryItem entity);
        Task<IEnumerable<MedicineDto>> GetByPharmacyIdAsync(int pharmacyId);
        Task<IEnumerable<MedicineDto>> GetByMedicationIdAsync(int medicationId);

        Task<InventoryItem?> GetByPharmacyAndMedicationAsync(int pharmacyId, int medicationId);
        Task<InventoryItem?> GetInventoryItemByIdAsync(int id);

    }

}
