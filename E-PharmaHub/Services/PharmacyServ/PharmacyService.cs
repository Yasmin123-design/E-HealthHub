using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.Services.FileStorageServ;
using E_PharmaHub.UnitOfWorkes;

namespace E_PharmaHub.Services.PharmacyServ
{
    public class PharmacyService : IPharmacyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorage;

        public PharmacyService(IUnitOfWork unitOfWork, IFileStorageService fileStorage)
        {
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
        }
        public async Task<(bool Success, string Message)> UpdatePharmacyAsync(int id, PharmacyUpdateDto dto, IFormFile? image)
        {
            var existing = await _unitOfWork.Pharmacies.GetByIdAsync(id);
            if (existing == null)
                return (false, "Pharmacy not found");

            if (!string.IsNullOrWhiteSpace(dto.Name))
                existing.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Phone))
                existing.Phone = dto.Phone;

            if (dto.AddressId.HasValue && dto.AddressId > 0)
                existing.AddressId = dto.AddressId.Value;

            if (image != null)
            {
                if (!string.IsNullOrEmpty(existing.ImagePath))
                    _fileStorage.DeleteFile(existing.ImagePath, "pharmacies");

                existing.ImagePath = await _fileStorage.SaveFileAsync(image, "pharmacies");
            }

            _unitOfWork.Pharmacies.Update(existing);
            await _unitOfWork.CompleteAsync();

            return (true, "Updated successfully");
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
