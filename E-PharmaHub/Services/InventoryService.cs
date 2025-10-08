using E_PharmaHub.Models;
using E_PharmaHub.UnitOfWorkes;

namespace E_PharmaHub.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InventoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<InventoryItem>> GetAlternativeMedicinesAsync(int medicineId)
        {
            return await _unitOfWork.IinventoryItem.GetAlternativeMedicinesAsync(medicineId);
        }
        public async Task AddInventoryItemAsync(InventoryItem item)
        {
            await _unitOfWork.IinventoryItem.AddAsync(item);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateInventoryItemAsync(InventoryItem item)
        {
            _unitOfWork.IinventoryItem.Update(item);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteInventoryItemAsync(int id)
        {
            var item = await _unitOfWork.IinventoryItem.GetByIdAsync(id);
            if (item != null)
            {
                _unitOfWork.IinventoryItem.Delete(item);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<InventoryItem> GetInventoryItemByIdAsync(int id)
        {
            return await _unitOfWork.IinventoryItem.GetByIdAsync(id);
        }

        public async Task<IEnumerable<InventoryItem>> GetInventoryByPharmacyAsync(int pharmacyId)
        {
            return await _unitOfWork.IinventoryItem.GetByPharmacyIdAsync(pharmacyId);
        }
    }

}
