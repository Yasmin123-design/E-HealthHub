using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.Helpers;
using Microsoft.EntityFrameworkCore;

namespace E_PharmaHub.Repositories.PharmacistRepo
{
    public class PharmacistRepository : IPharmacistRepository
    {
        private readonly EHealthDbContext _context;

        public PharmacistRepository(EHealthDbContext context)
        {
            _context = context;
        }

        private IQueryable<PharmacistProfile> BasePharmacistIncludes()
        {
            return _context.Pharmacists
                .AsNoTracking()
                .Include(p => p.AppUser)
                .Include(p => p.Pharmacy)
                .ThenInclude(ph => ph.Address);
        }

        public async Task<IEnumerable<PharmacistProfile>> GetAllAsync()
        {
            return await BasePharmacistIncludes().ToListAsync();
        }

        public async Task<PharmacistProfile> GetByIdAsync(int id)
        {
            return await BasePharmacistIncludes()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task MarkAsPaid(string userId)
        {
            var pharmacist = await _context.Pharmacists
                .FirstOrDefaultAsync(d => d.AppUserId == userId);

            if (pharmacist == null)
                throw new Exception("Doctor profile not found.");

            pharmacist.HasPaid = true;
        }
        public async Task<int?> GetPharmacyIdByUserIdAsync(string userId)
        {
            var profile = await _context.Pharmacists
                .FirstOrDefaultAsync(p => p.AppUserId == userId);

            return profile?.PharmacyId;
        }
        public async Task<PharmacistProfile?> GetByPharmacyIdAsync(int pharmacyId)
        {
            return await _context.Pharmacists
                .FirstOrDefaultAsync(p => p.PharmacyId == pharmacyId);
        }

        public async Task<IEnumerable<PharmacistReadDto>> GetAllDetailsAsync()
        {
            var pharmacists = await BasePharmacistIncludes().ToListAsync();
            return pharmacists.Select(PharmacistSelector.MapToDto).ToList();
        }

        public async Task<PharmacistReadDto?> GetByIdDetailsAsync(int id)
        {
            var pharmacist = await BasePharmacistIncludes()
                .FirstOrDefaultAsync(p => p.Id == id);

            return PharmacistSelector.MapToDto(pharmacist);
        }

        public async Task AddAsync(PharmacistProfile entity)
        {
            await _context.Pharmacists.AddAsync(entity);
        }

        public async Task Update(PharmacistProfile entity)
        {
            _context.Pharmacists.Update(entity);
        }

        public void Delete(PharmacistProfile entity)
        {
            _context.Pharmacists.Remove(entity);
        }

        public async Task<PharmacistReadDto?> GetPharmacistReadDtoByUserIdAsync(string userId)
        {
            var pharmacist = await BasePharmacistIncludes()
                .FirstOrDefaultAsync(p => p.AppUserId == userId);

            return PharmacistSelector.MapToDto(pharmacist);
        }


        public async Task<bool> ApprovePharmacistAsync(int id)
        {
            var pharmacist = await _context.Pharmacists.FindAsync(id);
            if (pharmacist == null || pharmacist.IsApproved)
                return false;

            pharmacist.IsApproved = true;
            pharmacist.IsRejected = false;
            return true;
        }

        public async Task<PharmacistProfile?> GetByUserIdAsync(string userId)
        {
            return await BasePharmacistIncludes()
                .FirstOrDefaultAsync(p => p.AppUserId == userId);
        }

        public async Task<bool> RejectPharmacistAsync(int id)
        {
            var pharmacist = await _context.Pharmacists.FindAsync(id);
            if (pharmacist == null || pharmacist.IsRejected)
                return false;

            pharmacist.IsRejected = true;
            pharmacist.IsApproved = false;
            return true;
        }
    }
}

