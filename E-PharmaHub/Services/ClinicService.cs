using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.UnitOfWorkes;
using Stripe;

namespace E_PharmaHub.Services
{
    public class ClinicService : IClinicService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;

        public ClinicService(IUnitOfWork unitOfWork,IFileStorageService fileStorage)
        {
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorage;
        }

        public async Task<Clinic> CreateClinicAsync(Clinic clinic)
        {
            await _unitOfWork.Clinics.AddAsync(clinic);
            await _unitOfWork.CompleteAsync();
            return clinic;
        }

        public async Task<Clinic?> GetClinicByIdAsync(int id)
        {
            return await _unitOfWork.Clinics.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Clinic>> GetAllClinicsAsync()
        {
            return await _unitOfWork.Clinics.GetAllAsync();
        }


        public async Task<bool> UpdateClinicAsync(string userId, ClinicUpdateDto dto, IFormFile? image)
        {
            var clinic = await _unitOfWork.Clinics.GetClinicByDoctorUserIdAsync(userId);
            if (clinic == null)
                return false;

            if (!string.IsNullOrEmpty(dto.Name))
                clinic.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Phone))
                clinic.Phone = dto.Phone;

            if (dto.AddressId.HasValue)
                clinic.AddressId = dto.AddressId.Value;

            if (image != null)
            {
                var imagePath = await _fileStorageService.SaveFileAsync(image, "clinics");
                clinic.ImagePath = imagePath;
            }

            _unitOfWork.Clinics.Update(clinic);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> DeleteClinicAsync(int id)
        {
            var clinic = await _unitOfWork.Clinics.GetByIdAsync(id);
            if (clinic == null) return false;

            _unitOfWork.Clinics.Delete(clinic);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }

}
