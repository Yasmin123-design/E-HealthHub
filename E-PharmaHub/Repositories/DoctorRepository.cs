using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace E_PharmaHub.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly EHealthDbContext _context;

        public DoctorRepository(EHealthDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DoctorProfile>> GetAllAsync()
        {
            return await _context.DoctorProfiles
                .Include(d => d.Clinic)
                .Include(d => d.AppUser)
                .ToListAsync();
        }

        public async Task<DoctorProfile> GetByIdAsync(int id)
        {
            return await _context.DoctorProfiles
                .Include(d => d.Clinic)
                .Include(d => d.AppUser)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task AddAsync(DoctorProfile entity)
        {
            await _context.DoctorProfiles.AddAsync(entity);
        }

        public void Update(DoctorProfile entity)
        {
            _context.DoctorProfiles.Update(entity);
        }
        public void Delete(DoctorProfile entity)
        {
            _context.DoctorProfiles.Remove(entity);
        }
        public async Task<IEnumerable<DoctorProfile>> GetDoctorsBySpecialtyAsync(string specialty)
        {
            return await _context.DoctorProfiles
                .Include(d => d.Clinic)
                .Include(d => d.AppUser)
                .Where(d => d.Specialty == specialty)
                .ToListAsync();
        }
    }
}
