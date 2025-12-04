using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Services.InventoryServ
{
    public interface IInventoryService
    {
        Task<IEnumerable<MedicineDto>> GetAlternativeMedicinesAsync(string name);
        Task AddInventoryItemAsync(InventoryItem item);
        Task UpdateInventoryItemAsync(InventoryItem item);
        Task DeleteInventoryItemAsync(int id);
        Task<MedicineDto> GetInventoryItemByIdAsync(int id);
        Task<IEnumerable<MedicineDto>> GetInventoryByPharmacyAsync(int pharmacyId);
    }

}
