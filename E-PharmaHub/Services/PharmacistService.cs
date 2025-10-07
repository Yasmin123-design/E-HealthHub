using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.Repositories;
using E_PharmaHub.UnitOfWorkes;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace E_PharmaHub.Services
{
    public class PharmacistService : IPharmacistService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileStorageService _fileStorage;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPharmacistRepository _pharmacistRepository;


        private readonly IUnitOfWork _unitOfWork;

        public PharmacistService(UserManager<AppUser> userManager,IPharmacistRepository pharmacistRepository, IHttpContextAccessor httpContextAccessor,IFileStorageService fileStorage, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _fileStorage = fileStorage;
            _httpContextAccessor = httpContextAccessor;
            _pharmacistRepository = pharmacistRepository;
        }

        public async Task<AppUser> RegisterPharmacistAsync(PharmacistRegisterDto dto, IFormFile image)
        {
            var user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                Role = UserRole.Pharmacist,
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, UserRole.Pharmacist.ToString());

            var address = new Address
            {
                Country = dto.Address.Country,
                City = dto.Address.City,
                Street = dto.Address.Street,
                PostalCode = dto.Address.PostalCode,
                Latitude = dto.Address.Latitude,
                Longitude = dto.Address.Longitude
            };
            await _unitOfWork.Addresses.AddAsync(address);
            await _unitOfWork.CompleteAsync();

            string? imagePath = null;
            if (image != null && image.Length > 0)
            {
                imagePath = await _fileStorage.SaveFileAsync(image, "pharmacies");
            }

            var pharmacy = new Pharmacy
            {
                Name = dto.PharmacyName,
                Phone = dto.PhoneNumber,
                AddressId = address.Id,
                ImagePath = imagePath
            };
            await _unitOfWork.Pharmacies.AddAsync(pharmacy);
            await _unitOfWork.CompleteAsync();

            var pharmacistProfile = new PharmacistProfile
            {
                AppUserId = user.Id,
                PharmacyId = pharmacy.Id,
                LicenseNumber = dto.LicenseNumber,
                IsApproved = false
            };
            await _unitOfWork.PharmasistsProfile.AddAsync(pharmacistProfile);
            await _unitOfWork.CompleteAsync();

            return user;
        }

        public async Task AddPharmacistAsync(PharmacistProfile pharmacist)
        {
            await _unitOfWork.PharmasistsProfile.AddAsync(pharmacist);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<PharmacistProfile>> GetAllPharmacistsAsync()
        {
            return await _unitOfWork.PharmasistsProfile.GetAllAsync();
        }

        public async Task<PharmacistProfile?> GetPharmacistByIdAsync(int id)
        {
            return await _unitOfWork.PharmasistsProfile.GetByIdAsync(id);
        }
        public async Task<PharmacistProfile?> GetPharmacistByUserIdAsync(string userId)
        {
            return await _pharmacistRepository.GetPharmacistByUserIdAsync(userId);
        }

        public async Task UpdatePharmacistAsync(int id, PharmacistProfile updatedPharmacist)
        {
            var existing = await _unitOfWork.PharmasistsProfile.GetByIdAsync(id);
            if (existing == null)
                throw new Exception("Pharmacist not found.");

            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated.");

            var isAdmin = _httpContextAccessor.HttpContext?.User?.IsInRole("Admin") ?? false;
            if (existing.AppUserId != userId && !isAdmin)
                throw new UnauthorizedAccessException("You are not allowed to update this pharmacist.");

            existing.LicenseNumber = updatedPharmacist.LicenseNumber;
            existing.PharmacyId = updatedPharmacist.PharmacyId;
            existing.AppUserId = userId;

            _unitOfWork.PharmasistsProfile.Update(existing);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeletePharmacistAsync(int id)
        {
            var pharmacist = await _unitOfWork.PharmasistsProfile.GetByIdAsync(id);
            if (pharmacist == null)
                throw new Exception("Pharmacist not found.");

            var user = await _userManager.FindByIdAsync(pharmacist.AppUserId);
            if (user != null)
                await _userManager.DeleteAsync(user);

            var pharmacy = await _unitOfWork.Pharmacies.GetByIdAsync(pharmacist.PharmacyId);
            if (pharmacy != null)
            {
                if (!string.IsNullOrEmpty(pharmacy.ImagePath))
                    _fileStorage.DeleteFile(pharmacy.ImagePath);

                _unitOfWork.Pharmacies.Delete(pharmacy);
            }

            _unitOfWork.PharmasistsProfile.Delete(pharmacist);
            await _unitOfWork.CompleteAsync();
        }

    }



}


