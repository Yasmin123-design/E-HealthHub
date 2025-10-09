using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace E_PharmaHub.Repositories
{
    public class PharmacistRepository : IPharmacistRepository
    {
        private readonly EHealthDbContext _context;

        public PharmacistRepository(EHealthDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PharmacistProfile>> GetAllAsync()
        {
            return await _context
                .Pharmacists
                .Include(p => p.AppUser)
                .Include(p => p.Pharmacy)
                .ToListAsync();
        }

        public async Task<PharmacistProfile> GetByIdAsync(int id)
        {
            return await _context
                .Pharmacists
                .Include(p => p.AppUser)
                .Include(p => p.Pharmacy)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<IEnumerable<PharmacistReadDto>> GetAllDetailsAsync()
        {
            return await _context.Pharmacists
                .Include(p => p.AppUser)
                .Include(p => p.Pharmacy)
                .ThenInclude(ph => ph.Address)
                .Select(p => new PharmacistReadDto
                {
                    Id = p.Id,
                    Email = p.AppUser.Email,
                    LicenseNumber = p.LicenseNumber,
                    IsApproved = p.IsApproved,
                    PharmacyName = p.Pharmacy.Name,
                    PharmacyPhone = p.Pharmacy.Phone,
                    PharmacyImagePath = p.Pharmacy.ImagePath,
                    City = p.Pharmacy.Address.City
                })
                .ToListAsync();
        }
        public async Task<PharmacistReadDto?> GetByIdDetailsAsync(int id)
        {
            return await _context.Pharmacists
                .Include(p => p.AppUser)
                .Include(p => p.Pharmacy)
                .ThenInclude(ph => ph.Address)
                .Where(p => p.Id == id)
                .Select(p => new PharmacistReadDto
                {
                    Id = p.Id,
                    Email = p.AppUser.Email,
                    LicenseNumber = p.LicenseNumber,
                    IsApproved = p.IsApproved,
                    PharmacyName = p.Pharmacy.Name,
                    PharmacyPhone = p.Pharmacy.Phone,
                    PharmacyImagePath = p.Pharmacy.ImagePath,
                    City = p.Pharmacy.Address.City
                })
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(PharmacistProfile entity)
        {
            await _context.Pharmacists.AddAsync(entity);
        }

        public void Update(PharmacistProfile entity)
        {
            _context.Pharmacists.Update(entity);
        }

        public void Delete(PharmacistProfile entity)
        {
            _context.Pharmacists.Remove(entity);
        }

        public async Task<PharmacistProfile?> GetPharmacistByUserIdAsync(string userId)
        {
            return await _context.Pharmacists
                .FirstOrDefaultAsync(p => p.AppUserId == userId);
        }
        public async Task<bool> ApprovePharmacistAsync(int id)
        {
            var pharmacist = await _context.Pharmacists.FindAsync(id);
            if (pharmacist == null || pharmacist.IsApproved)
                return false;

            pharmacist.IsApproved = true;
            return true;
        }

        public async Task<bool> RejectPharmacistAsync(int id)
        {
            var pharmacist = await _context.Pharmacists.FindAsync(id);
            if (pharmacist == null || !pharmacist.IsApproved)
                return false;

            pharmacist.IsApproved = false;
            return true;
        }
    }
}
