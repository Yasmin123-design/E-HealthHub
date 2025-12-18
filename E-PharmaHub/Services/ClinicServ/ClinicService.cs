using E_PharmaHub.Dtos;
using E_PharmaHub.Helpers;
using E_PharmaHub.Models;
using E_PharmaHub.Services.FileStorageServ;
using E_PharmaHub.UnitOfWorkes;

namespace E_PharmaHub.Services.ClinicServ
{
    public class ClinicService : IClinicService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;

        public ClinicService(IUnitOfWork unitOfWork, IFileStorageService fileStorage)
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

            var clinic = await _unitOfWork.Clinics
                .GetByIdAsync(doctor.ClinicId.Value);

            var doctorRelatedClinic = await _unitOfWork.Doctors.GetDoctorProfileByClinicIdAsync(clinic.Id);

            if (clinic == null)
                return (false, "Clinic not found ❌");

            if (!string.IsNullOrWhiteSpace(dto.Name))
                clinic.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Phone))
                clinic.Phone = dto.Phone;

            if (clinic.Address == null)
            {
                clinic.Address = new Address();
            }

            if (!string.IsNullOrWhiteSpace(dto.Country))
                clinic.Address.Country = dto.Country;

            if (!string.IsNullOrWhiteSpace(dto.City))
                clinic.Address.City = dto.City;

            if (!string.IsNullOrWhiteSpace(dto.Street))
                clinic.Address.Street = dto.Street;

            if (!string.IsNullOrWhiteSpace(dto.PostalCode))
                clinic.Address.PostalCode = dto.PostalCode;

            if (dto.Latitude.HasValue)
                clinic.Address.Latitude = dto.Latitude;

            if (dto.Longitude.HasValue)
                clinic.Address.Longitude = dto.Longitude;

            if (dto.ConsultationPrice.HasValue)
                doctorRelatedClinic.ConsultationPrice = dto.ConsultationPrice.Value;

            if (dto.ConsultationType.HasValue)
                doctorRelatedClinic.ConsultationType = dto.ConsultationType.Value;


            if (image != null)
            {
                var imagePath = await _fileStorageService.SaveFileAsync(image, "clinics");
                clinic.ImagePath = imagePath;
            }

            _unitOfWork.Clinics.Update(clinic);
            _unitOfWork.Doctors.Update(doctorRelatedClinic);
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
