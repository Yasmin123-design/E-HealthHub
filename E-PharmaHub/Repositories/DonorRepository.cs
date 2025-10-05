using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace E_PharmaHub.Repositories
{
    public class DonorRepository : IGenericRepository<DonorProfile>
    {
        private readonly EHealthDbContext _context;

        public DonorRepository(EHealthDbContext context)
        {
            _context = context;
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
