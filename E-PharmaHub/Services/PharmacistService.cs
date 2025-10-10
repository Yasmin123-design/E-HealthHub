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
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new Exception("This email is already registered. Please use another one.");

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

            var existingAddress = await _unitOfWork.Addresses.FindAsync(a =>
                a.Country == dto.Address.Country &&
                a.City == dto.Address.City &&
                a.Street == dto.Address.Street &&
                a.PostalCode == dto.Address.PostalCode &&
                a.Latitude == dto.Address.Latitude &&
                a.Longitude == dto.Address.Longitude
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
                    Country = dto.Address.Country,
                    City = dto.Address.City,
                    Street = dto.Address.Street,
                    PostalCode = dto.Address.PostalCode,
                    Latitude = dto.Address.Latitude,
                    Longitude = dto.Address.Longitude
                };
                await _unitOfWork.Addresses.AddAsync(address);
                await _unitOfWork.CompleteAsync();
            }

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
                IsApproved = false,
                HasPaid = false  

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
        public async Task MarkAsPaid(string userId)
        {
            await _unitOfWork.PharmasistsProfile.MarkAsPaid(userId);
            await _unitOfWork.CompleteAsync();
        }
        public async Task<PharmacistProfile?> GetPharmacistByUserIdAsync(string userId)
        {
            return await _pharmacistRepository.GetPharmacistByUserIdAsync(userId);
        }
        public async Task<bool> ApprovePharmacistAsync(int id)
        {
            var result = await _unitOfWork.PharmasistsProfile.ApprovePharmacistAsync(id);

            if (!result)
                return false;

            await _unitOfWork.CompleteAsync(); 
            return true;
        }

        public async Task<bool> RejectPharmacistAsync(int id)
        {
            var result = await _unitOfWork.PharmasistsProfile.RejectPharmacistAsync(id);

            if (!result)
                return false;

            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task UpdatePharmacistAsync(int id, PharmacistProfile updatedPharmacist, IFormFile? newImage)
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

            var pharmacy = await _unitOfWork.Pharmacies.GetByIdAsync(existing.PharmacyId)
                           ?? throw new Exception("Pharmacy not found.");

            var currentAddress = await _unitOfWork.Addresses.GetByIdAsync(pharmacy.AddressId)
                                ?? throw new Exception("Address not found.");

            Address address = currentAddress;

            if (updatedPharmacist.Pharmacy?.Address != null)
            {
                var dtoAddress = updatedPharmacist.Pharmacy.Address;

                var existingAddress = await _unitOfWork.Addresses.FindAsync(a =>
                    a.Country == dtoAddress.Country &&
                    a.City == dtoAddress.City &&
                    a.Street == dtoAddress.Street &&
                    a.PostalCode == dtoAddress.PostalCode &&
                    a.Latitude == dtoAddress.Latitude &&
                    a.Longitude == dtoAddress.Longitude
                );

                if (existingAddress != null)
                {
                    address = existingAddress;
                }
                else
                {
                    address = new Address
                    {
                        Country = dtoAddress.Country ?? currentAddress.Country,
                        City = dtoAddress.City ?? currentAddress.City,
                        Street = dtoAddress.Street ?? currentAddress.Street,
                        PostalCode = dtoAddress.PostalCode ?? currentAddress.PostalCode,
                        Latitude = dtoAddress.Latitude ?? currentAddress.Latitude,
                        Longitude = dtoAddress.Longitude ?? currentAddress.Longitude
                    };
                    await _unitOfWork.Addresses.AddAsync(address);
                    await _unitOfWork.CompleteAsync();
                }
            }

            string imagePath = pharmacy.ImagePath;
            if (newImage != null && newImage.Length > 0)
            {
                imagePath = await _fileStorage.SaveFileAsync(newImage, "pharmacies");
            }

            pharmacy.Name = updatedPharmacist.Pharmacy?.Name ?? pharmacy.Name;
            pharmacy.Phone = updatedPharmacist.Pharmacy?.Phone ?? pharmacy.Phone;
            pharmacy.AddressId = address.Id;
            pharmacy.ImagePath = imagePath ?? pharmacy.ImagePath;

            _unitOfWork.Pharmacies.Update(pharmacy);

            existing.LicenseNumber = updatedPharmacist.LicenseNumber ?? existing.LicenseNumber;
            existing.PharmacyId = pharmacy.Id;

            if (isAdmin)
                existing.IsApproved = updatedPharmacist.IsApproved;

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

        public async Task<IEnumerable<PharmacistReadDto>> GetAllPharmacistsAsync()
        {
            return await _unitOfWork.PharmasistsProfile.GetAllDetailsAsync();
        }

        public async Task<PharmacistReadDto?> GetPharmacistByIdAsync(int id)
        {
            return await _unitOfWork.PharmasistsProfile.GetByIdDetailsAsync(id);
        }
        public async Task<PharmacistProfile?> GetPharmacistProfileByIdAsync(int id)
        {
            return await _unitOfWork.PharmasistsProfile.GetByIdAsync(id);
        }
    }



}


