using E_PharmaHub.Dtos;
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
        public async Task<(bool Success, string Message)> UpdatePharmacyAsync(string userId, PharmacyUpdateDto dto, IFormFile? image)
        {
            var pharmacy = await _unitOfWork.Pharmacies.GetPharmacyByPharmacistUserIdAsync(userId);
            if (pharmacy == null)
                return (false, "Pharmacy not found.");

            if (!string.IsNullOrEmpty(dto.Name))
                pharmacy.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Phone))
                pharmacy.Phone = dto.Phone;

            if (dto.AddressId.HasValue)
            {
                var addressExists = await _unitOfWork.Addresses.GetByIdAsync(dto.AddressId.Value);
                if (addressExists == null)
                    return (false, "The provided address does not exist ❌");

                pharmacy.AddressId = dto.AddressId.Value;
            }

            if (image != null)
            {
                var imagePath = await _fileStorage.SaveFileAsync(image, "pharmacies");
                pharmacy.ImagePath = imagePath;
            }

            _unitOfWork.Pharmacies.Update(pharmacy);
            await _unitOfWork.CompleteAsync();

            return (true, "Pharmacy updated successfully ✅");
        }


        public async Task<IEnumerable<PharmacySimpleDto>> GetAllPharmaciesAsync()
        {
            var pharmacies = await _unitOfWork.Pharmacies.GetAllBriefAsync();
            return pharmacies ?? Enumerable.Empty<PharmacySimpleDto>();
        }

        public async Task<PharmacySimpleDto> GetPharmacyByIdAsync(int id)
        {
            return await _unitOfWork.Pharmacies.GetByIdBriefAsync(id);
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

        public async Task<IEnumerable<PharmacySimpleDto>> GetNearestPharmaciesWithMedicationAsync(string medicationName, double userLat, double userLng)
        {
            return await _unitOfWork.Pharmacies.GetNearestPharmaciesWithMedicationAsync(medicationName, userLat, userLng);

        }

        public async Task<IEnumerable<PharmacySimpleDto>> GetTopRatedPharmaciesAsync()
        {
            var pharmacies = await _unitOfWork.Pharmacies.GetTopRatedPharmaciesAsync(3);
            return pharmacies;

        }
    }

}
