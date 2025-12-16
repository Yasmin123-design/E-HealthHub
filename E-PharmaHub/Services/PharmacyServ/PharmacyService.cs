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
        public async Task<(bool Success, string Message)> UpdatePharmacyAsync(
    int id,
    PharmacyUpdateDto dto,
    IFormFile? image)
        {
            var pharmacy = await _unitOfWork.Pharmacies
                .GetByIdAsync(id);

            if (pharmacy == null)
                return (false, "Pharmacy not found ❌");

            if (!string.IsNullOrWhiteSpace(dto.Name))
                pharmacy.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Phone))
                pharmacy.Phone = dto.Phone;

            if (
                dto.Country != null ||
                dto.City != null ||
                dto.Street != null ||
                dto.PostalCode != null ||
                dto.Latitude.HasValue ||
                dto.Longitude.HasValue
            )
            {
                if (pharmacy.Address == null)
                {
                    pharmacy.Address = new Address
                    {
                        Country = dto.Country ?? "",
                        City = dto.City ?? "",
                        Street = dto.Street ?? "",
                        PostalCode = dto.PostalCode,
                        Latitude = dto.Latitude,
                        Longitude = dto.Longitude
                    };
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(dto.Country))
                        pharmacy.Address.Country = dto.Country;

                    if (!string.IsNullOrWhiteSpace(dto.City))
                        pharmacy.Address.City = dto.City;

                    if (!string.IsNullOrWhiteSpace(dto.Street))
                        pharmacy.Address.Street = dto.Street;

                    if (!string.IsNullOrWhiteSpace(dto.PostalCode))
                        pharmacy.Address.PostalCode = dto.PostalCode;

                    if (dto.Latitude.HasValue)
                        pharmacy.Address.Latitude = dto.Latitude;

                    if (dto.Longitude.HasValue)
                        pharmacy.Address.Longitude = dto.Longitude;
                }
            }

            if (image != null)
            {
                if (!string.IsNullOrEmpty(pharmacy.ImagePath))
                    _fileStorage.DeleteFile(pharmacy.ImagePath, "pharmacies");

                pharmacy.ImagePath = await _fileStorage.SaveFileAsync(image, "pharmacies");
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
