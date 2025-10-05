using E_PharmaHub.Models;
using E_PharmaHub.UnitOfWorkes;

namespace E_PharmaHub.Services
{
    public class PharmacyService : IPharmacyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorage;

        public PharmacyService(IUnitOfWork unitOfWork , IFileStorageService fileStorage)
        {
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
        }

        public async Task<IEnumerable<Pharmacy>> GetAllPharmaciesAsync()
        {
            var pharmacies = await _unitOfWork.Pharmacies.GetAllAsync();
            return pharmacies ?? Enumerable.Empty<Pharmacy>();
        }

        public async Task<Pharmacy> GetPharmacyByIdAsync(int id)
        {
            return await _unitOfWork.Pharmacies.GetByIdAsync(id);
        }

        public async Task AddPharmacyAsync(Pharmacy pharmacy, IFormFile imageFile)
        {
            if (imageFile != null)
            {
                pharmacy.ImagePath = await _fileStorage.SaveFileAsync(imageFile, "pharmacies");
            }

            await _unitOfWork.Pharmacies.AddAsync(pharmacy);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdatePharmacyAsync(int id , Pharmacy updatedPharmacy, IFormFile imageFile)
        {
            var existing = await _unitOfWork.Pharmacies.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException("Pharmacy not found");

            existing.Name = updatedPharmacy.Name;
            existing.Phone = updatedPharmacy.Phone;
            existing.AddressId = updatedPharmacy.AddressId;
            if (updatedPharmacy.Address != null && updatedPharmacy.Address.Id != 0)
            {
                var address = await _unitOfWork.Addresses.GetByIdAsync(updatedPharmacy.Address.Id);
                if (address != null)
                    existing.Address = address;
            }


            if (imageFile != null)
            {
                if (!string.IsNullOrEmpty(existing.ImagePath))
                    _fileStorage.DeleteFile(existing.ImagePath);

                existing.ImagePath = await _fileStorage.SaveFileAsync(imageFile, "pharmacies");
            }
            _unitOfWork.Pharmacies.Update(existing);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeletePharmacyAsync(int id)
        {
            var pharmacy = await _unitOfWork.Pharmacies.GetByIdAsync(id);
            if (pharmacy != null)
            {
                _unitOfWork.Pharmacies.Delete(pharmacy);
                await _unitOfWork.CompleteAsync();
            }
        }
    }

}
