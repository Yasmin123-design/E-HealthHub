using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.UnitOfWorkes;
using Microsoft.AspNetCore.Identity;

namespace E_PharmaHub.Services
{
    public class DonorService : IDonorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public DonorService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IEnumerable<DonorReadDto>> GetAllDetailsAsync()
        {
            return await _unitOfWork.Donors.GetAllDetailsAsync();
        }

        public async Task<IEnumerable<DonorReadDto>> GetByFilterAsync(BloodType? type, string? city)
        {
            return await _unitOfWork.Donors.GetByFilterAsync(type, city);
        }

        public async Task<DonorProfile?> GetByUserIdAsync(string userId)
        {
            return await _unitOfWork.Donors.GetByUserIdAsync(userId);
        }

        public async Task<DonorProfile> RegisterAsync(DonorRegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new Exception("This email is already registered.");

            var user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                Role = UserRole.Donor
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));

            var donor = new DonorProfile
            {
                AppUserId = user.Id,
                BloodType = dto.BloodType,
                City = dto.City,
                IsAvailable = true
            };

            await _unitOfWork.Donors.AddAsync(donor);
            await _unitOfWork.CompleteAsync();

            return donor;
        }


        public async Task<bool> UpdateAvailabilityAsync(string userId, bool isAvailable)
        {
            var result = await _unitOfWork.Donors.UpdateAvailabilityAsync(userId, isAvailable);
            await _unitOfWork.CompleteAsync();
            return result;
        }
        public async Task UpdateAsync(DonorProfile donor)
        {
            _unitOfWork.Donors.Update(donor);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var donor = await _unitOfWork.Donors.GetByIdAsync(id);
            if (donor == null)
                throw new Exception("Donor not found");

            var user = await _userManager.FindByIdAsync(donor.AppUserId);

            _unitOfWork.Donors.Delete(donor);
            await _unitOfWork.CompleteAsync();

            if (user != null)
                await _userManager.DeleteAsync(user);
        }
    }
}
