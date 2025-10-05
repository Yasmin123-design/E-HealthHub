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

        public async Task UpdateMedicineAsync(int id , Medication medicine, IFormFile? imageFile)
        {
            var existing = await _unitOfWork.Medicines.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Medicine not found.");
            existing.DosageForm = medicine.DosageForm;
            existing.ATCCode = medicine.ATCCode;
            existing.GenericName = medicine.GenericName;
            existing.BrandName = medicine.BrandName;
            existing.Strength = medicine.Strength;
            if (imageFile != null)
            {
                // احذف الصورة القديمة لو موجودة
                if (!string.IsNullOrEmpty(existing.ImagePath))
                    _fileStorage.DeleteFile(existing.ImagePath);

                medicine.ImagePath = await _fileStorage.SaveFileAsync(imageFile, "medicines");
            }
            else
            {
                medicine.ImagePath = existing.ImagePath; // يحتفظ بالقديمة
            }

            _unitOfWork.Medicines.Update(existing);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteMedicineAsync(int id)
        {
            var existing = await _unitOfWork.Medicines.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Medicine not found.");

            if (!string.IsNullOrEmpty(existing.ImagePath))
                _fileStorage.DeleteFile(existing.ImagePath);

            _unitOfWork.Medicines.Delete(existing);
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
    }

}
