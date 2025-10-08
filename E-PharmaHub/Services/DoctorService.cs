using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.Repositories;
using E_PharmaHub.UnitOfWorkes;
using Microsoft.AspNetCore.Identity;

namespace E_PharmaHub.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileStorageService _fileStorage;
        private readonly IDoctorRepository _doctorRepository;

        public DoctorService(IUnitOfWork unitOfWork,IDoctorRepository doctorRepository, UserManager<AppUser> userManager,IFileStorageService fileStorage)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _fileStorage = fileStorage;
            _doctorRepository = doctorRepository;
        }
        public async Task<DoctorProfile?> GetDoctorByUserIdAsync(string userId)
        {
            return await _doctorRepository.GetDoctorByUserIdAsync(userId);
        }
        public async Task<AppUser> RegisterDoctorAsync(DoctorRegisterDto dto, IFormFile image)
        {
            var user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                Role = UserRole.Doctor
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, UserRole.Doctor.ToString());

            var existingAddress = await _unitOfWork.Addresses.FindAsync(a =>
                a.Country == dto.ClinicAddress.Country &&
                a.City == dto.ClinicAddress.City &&
                a.Street == dto.ClinicAddress.Street &&
                a.PostalCode == dto.ClinicAddress.PostalCode &&
                a.Latitude == dto.ClinicAddress.Latitude &&
                a.Longitude == dto.ClinicAddress.Longitude
            );

            Address address;
            if (existingAddress != null)
            {
                address = existingAddress;
            }
            else
            {
                address = new Address
                {
                    Country = dto.ClinicAddress.Country,
                    City = dto.ClinicAddress.City,
                    Street = dto.ClinicAddress.Street,
                    PostalCode = dto.ClinicAddress.PostalCode,
                    Latitude = dto.ClinicAddress.Latitude,
                    Longitude = dto.ClinicAddress.Longitude
                };
                await _unitOfWork.Addresses.AddAsync(address);
                await _unitOfWork.CompleteAsync();
            }

            string imagePath = null;
            if (image != null)
            {
                imagePath = await _fileStorage.SaveFileAsync(image, "clinics");
            }

            var clinic = new Clinic
            {
                Name = dto.ClinicName,
                Phone = dto.ClinicPhone,
                AddressId = address.Id,
                ImagePath = imagePath
            };

            await _unitOfWork.Clinics.AddAsync(clinic);
            await _unitOfWork.CompleteAsync();

            var doctorProfile = new DoctorProfile
            {
                AppUserId = user.Id,
                ClinicId = clinic.Id,
                Specialty = dto.Specialty,
                IsApproved = false
            };

            await _unitOfWork.Doctors.AddAsync(doctorProfile);
            await _unitOfWork.CompleteAsync();

            return user;
        }


        public async Task<DoctorProfile?> GetDoctorByIdAsync(int id)
        {
            return await _unitOfWork.Doctors.GetByIdAsync(id);
        }
        public async Task<bool> ApproveDoctorAsync(int id)
        {
            return await _unitOfWork.Doctors.ApproveDoctorAsync(id);
        }

        public async Task<bool> RejectDoctorAsync(int id)
        {
            return await _unitOfWork.Doctors.RejectDoctorAsync(id);
        }
        public async Task<IEnumerable<DoctorProfile>> GetDoctorsBySpecialtyAsync(string specialty)
        {
            return await _unitOfWork.Doctors.GetDoctorsBySpecialtyAsync(specialty);
        }

        public async Task UpdateDoctorAsync(int id, DoctorProfile updatedDoctor, IFormFile? newImage)
        {
            var existing = await _unitOfWork.Doctors.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Doctor not found.");

            var clinic = await _unitOfWork.Clinics.GetByIdAsync(existing.ClinicId);
            if (clinic == null)
                throw new Exception("Clinic not found.");

            var address = await _unitOfWork.Addresses.GetByIdAsync(clinic.AddressId);
            if (address == null)
                throw new Exception("Clinic address not found.");

            existing.Specialty = updatedDoctor.Specialty ?? existing.Specialty;

            clinic.Name = updatedDoctor.Clinic?.Name ?? clinic.Name;
            clinic.Phone = updatedDoctor.Clinic?.Phone ?? clinic.Phone;

            if (updatedDoctor.Clinic?.Address != null)
            {
                address.Country = updatedDoctor.Clinic.Address.Country ?? address.Country;
                address.City = updatedDoctor.Clinic.Address.City ?? address.City;
                address.Street = updatedDoctor.Clinic.Address.Street ?? address.Street;
                address.PostalCode = updatedDoctor.Clinic.Address.PostalCode ?? address.PostalCode;
                address.Latitude = updatedDoctor.Clinic.Address.Latitude ?? address.Latitude;
                address.Longitude = updatedDoctor.Clinic.Address.Longitude ?? address.Longitude;
            }

            if (newImage != null)
            {
                if (!string.IsNullOrEmpty(clinic.ImagePath))
                    _fileStorage.DeleteFile(clinic.ImagePath);

                clinic.ImagePath = await _fileStorage.SaveFileAsync(newImage, "clinics");
            }

            _unitOfWork.Doctors.Update(existing);
            _unitOfWork.Clinics.Update(clinic);
            _unitOfWork.Addresses.Update(address);

            await _unitOfWork.CompleteAsync();
        }
        public async Task DeleteDoctorAsync(int id)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(id);
            if (doctor == null)
                throw new Exception("Doctor not found.");

            var clinic = await _unitOfWork.Clinics.GetByIdAsync(doctor.ClinicId);
            if (clinic != null)
            {
                if (!string.IsNullOrEmpty(clinic.ImagePath))
                    _fileStorage.DeleteFile(clinic.ImagePath);

                _unitOfWork.Clinics.Delete(clinic);
            }

            _unitOfWork.Doctors.Delete(doctor);

            if (!string.IsNullOrEmpty(doctor.AppUserId))
            {
                var user = await _userManager.FindByIdAsync(doctor.AppUserId);
                if (user != null)
                {
                    var result = await _userManager.DeleteAsync(user);
                    if (!result.Succeeded)
                        throw new Exception("Failed to delete user account.");
                }
            }

            await _unitOfWork.CompleteAsync();
        }

        //public async Task<bool> ApproveDoctorAsync(string doctorUserId)
        //{
        //    var doctor = await _unitOfWork.DoctorRepository.GetDoctorWithClinicAsync(doctorUserId);
        //    if (doctor == null) return false;

        //    var user = await _userManager.FindByIdAsync(doctorUserId);
        //    if (user == null) return false;

        //    user.IsApproved = true;
        //    var result = await _userManager.UpdateAsync(user);
        //    return result.Succeeded;
        //}
    }

}
