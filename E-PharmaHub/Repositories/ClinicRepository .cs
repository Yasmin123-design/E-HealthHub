using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace E_PharmaHub.Repositories
{
    public class ClinicRepository : IClinicRepository
    {
        private readonly EHealthDbContext _context;

        public ClinicRepository(EHealthDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Clinic>> GetAllAsync()
        {
            return await _context.Clinics
                .Include(c => c.Address)
                .ToListAsync();
        }

        public async Task<Clinic> GetByIdAsync(int id)
        {
            return await _context.Clinics
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Clinic entity)
        {
            await _context.Clinics.AddAsync(entity);
        }

        public async Task Update(Clinic entity)
        {
            _context.Clinics.Update(entity);
        }

        public void Delete(Clinic entity)
        {
            _context.Clinics.Remove(entity);
        }

        public async Task<Clinic> GetClinicByIdAsync(int? id)
        {
            return await _context.Clinics
                         .Include(c => c.Address)
                         .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<Clinic?> GetClinicByDoctorUserIdAsync(string userId)
        {
            return await _context.DoctorProfiles
                .Where(d => d.AppUserId == userId)
                .Select(d => d.Clinic)
                .FirstOrDefaultAsync();
        }

    }
}
