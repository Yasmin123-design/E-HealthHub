using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.UnitOfWorkes;

namespace E_PharmaHub.Services
{
    public class MedicineService : IMedicineService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorage;

        public MedicineService(IUnitOfWork unitOfWork, IFileStorageService fileStorage)
        {
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
        }

        public async Task<IEnumerable<Medication>> GetAllMedicinesAsync()
        {
            return await _unitOfWork.Medicines.GetAllAsync();
        }

        public async Task<Medication> GetMedicineByIdAsync(int id)
        {
            return await _unitOfWork.Medicines.GetByIdAsync(id);
        }


        public async Task AddMedicineAsync(Medication medicine, IFormFile? imageFile)
        {
            if (imageFile != null)
            {
                medicine.ImagePath = await _fileStorage.SaveFileAsync(imageFile, "medicines");
            }

            await _unitOfWork.Medicines.AddAsync(medicine);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateMedicineAsync(int id, MedicineDto dto, IFormFile? image, int? pharmacyId)
        {
            var existingMedicine = await _unitOfWork.Medicines.GetByIdAsync(id)
                ?? throw new Exception("Medicine not found.");

            existingMedicine.BrandName = dto.BrandName ?? existingMedicine.BrandName;
            existingMedicine.GenericName = dto.GenericName ?? existingMedicine.GenericName;
            existingMedicine.DosageForm = dto.DosageForm ?? existingMedicine.DosageForm;
            existingMedicine.Strength = dto.Strength ?? existingMedicine.Strength;
            existingMedicine.ATCCode = dto.ATCCode ?? existingMedicine.ATCCode;

            if (image != null && image.Length > 0)
            {
                existingMedicine.ImagePath = await _fileStorage.SaveFileAsync(image, "medicines");
            }

            _unitOfWork.Medicines.Update(existingMedicine);

            if (pharmacyId.HasValue)
            {
                var inventory = await _unitOfWork.IinventoryItem.FindAsync(i =>
                    i.MedicationId == id && i.PharmacyId == pharmacyId.Value);

                if (inventory != null)
                {
                    inventory.Price = dto.Price ?? inventory.Price;
                    inventory.Quantity = dto.Quantity ?? inventory.Quantity;
                    inventory.LastUpdated = DateTime.UtcNow;

                    _unitOfWork.IinventoryItem.Update(inventory);
                }
            }

            await _unitOfWork.CompleteAsync();
        }


        public async Task DeleteMedicineAsync(int id, int? pharmacyId)
        {
            if (pharmacyId.HasValue)
            {
                var inventory = await _unitOfWork.IinventoryItem.FindAsync(i =>
                    i.MedicationId == id && i.PharmacyId == pharmacyId.Value);

                if (inventory == null)
                    throw new Exception("Medicine not found in your pharmacy inventory.");

                _unitOfWork.IinventoryItem.Delete(inventory);
            }
            else
            {
                var medicine = await _unitOfWork.Medicines.GetByIdAsync(id)
                    ?? throw new Exception("Medicine not found.");

                var inventories = await _unitOfWork.IinventoryItem.FindAllAsync(i => i.MedicationId == id);
                foreach (var inv in inventories)
                {
                    _unitOfWork.IinventoryItem.Delete(inv);
                }

                _unitOfWork.Medicines.Delete(medicine);
            }

            await _unitOfWork.CompleteAsync();
        }


        public async Task<IEnumerable<Medication>> SearchMedicinesByNameAsync(string name)
        {
            return await _unitOfWork.Medicines.SearchByNameAsync(name);
        }

        public async Task<IEnumerable<Medication>> GetMedicinesByPharmacyIdAsync(int pharmacyId)
        {
            return await _unitOfWork.Medicines.GetMedicinesByPharmacyIdAsync(pharmacyId);
        }

        public async Task<IEnumerable<Pharmacy>> GetNearestPharmaciesWithMedicationAsync(string medicationName, double userLat, double userLng)
        {
            return await _unitOfWork.Medicines.GetNearestPharmaciesWithMedicationAsync(medicationName, userLat, userLng);

        }
        public async Task AddMedicineWithInventoryAsync(MedicineDto dto, IFormFile? image, int pharmacyId)
        {
            var medicine = new Medication
            {
                BrandName = dto.BrandName,
                GenericName = dto.GenericName,
                DosageForm = dto.DosageForm,
                Strength = dto.Strength,
                ATCCode = dto.ATCCode
            };

            if (image != null && image.Length > 0)
                medicine.ImagePath = await _fileStorage.SaveFileAsync(image, "medicines");

            await _unitOfWork.Medicines.AddAsync(medicine);
            await _unitOfWork.CompleteAsync();

            var inventoryItem = new InventoryItem
            {
                PharmacyId = pharmacyId,
                MedicationId = medicine.Id,
                Price = dto.Price,
                Quantity = dto.Quantity,
                LastUpdated = DateTime.UtcNow
            };

            await _unitOfWork.IinventoryItem.AddAsync(inventoryItem);
            await _unitOfWork.CompleteAsync();
        }

    }

}
