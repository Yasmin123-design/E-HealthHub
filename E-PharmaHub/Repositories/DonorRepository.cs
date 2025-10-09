using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace E_PharmaHub.Repositories
{
    public class DonorRepository : IDonorRepository
    {
        private readonly EHealthDbContext _context;

        public DonorRepository(EHealthDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<DonorProfile>> GetByFilterAsync(BloodType? type, string? city)
        {
            var query = _context.DonorProfiles.AsQueryable();

            if (type.HasValue)
                query = query.Where(d => d.BloodType == type.Value);

            if (!string.IsNullOrEmpty(city))
                query = query.Where(d => d.City.ToLower() == city.ToLower());

            return await query.Include(d => d.AppUser).ToListAsync();
        }

        public async Task<DonorProfile?> GetByUserIdAsync(string userId)
        {
            return await _context.DonorProfiles
                .Include(d => d.AppUser)
                .FirstOrDefaultAsync(d => d.AppUserId == userId);
        }

        public async Task<bool> UpdateAvailabilityAsync(string userId, bool isAvailable)
        {
            var donor = await _context.DonorProfiles.FirstOrDefaultAsync(d => d.AppUserId == userId);
            if (donor == null) return false;

            donor.IsAvailable = isAvailable;
            return true;
        }
        public async Task<IEnumerable<DonorProfile>> GetAllAsync()
        {
            return await _context.DonorProfiles
                .Include(d => d.AppUser)
                .ToListAsync();
        }

        public async Task<DonorProfile> GetByIdAsync(int id)
        {
            return await _context.DonorProfiles
                .Include(d => d.AppUser)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task AddAsync(DonorProfile entity)
        {
            await _context.DonorProfiles.AddAsync(entity);
        }

        public void Update(DonorProfile entity)
        {
            _context.DonorProfiles.Update(entity);
        }

        public void Delete(DonorProfile entity)
        {
            _context.DonorProfiles.Remove(entity);
        }

    }
}
