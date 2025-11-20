using E_PharmaHub.Dtos;
using E_PharmaHub.Helpers;
using E_PharmaHub.Models;
using E_PharmaHub.UnitOfWorkes;

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

        public async Task<ClinicDto?> GetClinicDtoByIdAsync(int id)
        {
            return await _unitOfWork.Clinics.GetDtoByIdAsync(id);
        }

        public async Task<IEnumerable<Clinic>> GetAllClinicsAsync()
        {
            return await _unitOfWork.Clinics.GetAllAsync();
        }

        public async Task<IEnumerable<ClinicDto>> GetAllClinicsDtoAsync()
        {
            return await _unitOfWork.Clinics.GetAllDtoAsync();
        }

        public async Task<(bool Success, string Message)> UpdateClinicAsync(
            string userId, ClinicUpdateDto dto, IFormFile? image)
        {
            var doctor = await _unitOfWork.Doctors.GetDoctorByUserIdAsync(userId);
            if (doctor == null)
                return (false, "Doctor profile not found ❌");

            if (doctor.ClinicId == null)
                return (false, "Doctor does not have an assigned clinic ❌");

            var clinic = await _unitOfWork.Clinics.GetByIdAsync(doctor.ClinicId.Value);
            if (clinic == null)
                return (false, "Clinic not found ❌");

            if (!string.IsNullOrEmpty(dto.Name))
                clinic.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Phone))
                clinic.Phone = dto.Phone;

            if (dto.AddressId.HasValue)
            {
                var addressExists = await _unitOfWork.Addresses.GetByIdAsync(dto.AddressId.Value);
                if (addressExists == null)
                    return (false, "The provided address does not exist ❌");

                clinic.AddressId = dto.AddressId.Value;
            }

            if (image != null)
            {
                var imagePath = await _fileStorageService.SaveFileAsync(image, "clinics");
                clinic.ImagePath = imagePath;
            }
            _unitOfWork.Clinics.Update(clinic);
            await _unitOfWork.CompleteAsync();

            return (true, "Clinic updated successfully ✅");
        }

        public async Task<bool> DeleteClinicAsync(int id)
        {
            var clinic = await _unitOfWork.Clinics.GetByIdAsync(id);
            if (clinic == null) return false;

            _unitOfWork.Clinics.Delete(clinic);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<Clinic?> GetClinicByDoctorUserIdAsync(string userId)
        {
            return await _unitOfWork.Clinics.GetClinicByDoctorUserIdAsync(userId);
        }

        public async Task<ClinicDto?> GetClinicDtoByDoctorUserIdAsync(string userId)
        {
            var clinic = await _unitOfWork.Clinics.GetClinicByDoctorUserIdAsync(userId);
            return clinic == null ? null : ClinicSelectors.MapToDto(clinic);
        }
    }

}
