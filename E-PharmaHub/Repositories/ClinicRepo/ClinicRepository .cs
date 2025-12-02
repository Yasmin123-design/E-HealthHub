using E_PharmaHub.Dtos;
using E_PharmaHub.Helpers;
using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace E_PharmaHub.Repositories.ClinicRepo
{
    public class ClinicRepository : IClinicRepository
    {
        private readonly EHealthDbContext _context;

        public ClinicRepository(EHealthDbContext context)
        {
            _context = context;
        }

        private IQueryable<Clinic> BaseClinicIncludes()
        {
            return _context.Clinics
                .Include(c => c.Address)
                .AsNoTracking()
                .Where(c => _context.DoctorProfiles
                    .Any(d => d.ClinicId == c.Id && d.IsApproved));
        }

        private Expression<Func<Clinic, ClinicDto>> Selector => ClinicSelectors.ClinicDtoSelector;

        public async Task<IEnumerable<Clinic>> GetAllAsync()
        {
            return await BaseClinicIncludes().ToListAsync();
        }

        public async Task<IEnumerable<ClinicDto>> GetAllDtoAsync()
        {
            return await BaseClinicIncludes()
                .Select(Selector)
                .ToListAsync();
        }

        public async Task<Clinic?> GetByIdAsync(int id)
        {
            return await BaseClinicIncludes()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<ClinicDto?> GetDtoByIdAsync(int id)
        {
            return await BaseClinicIncludes()
                .Where(c => c.Id == id)
                .Select(Selector)
                .FirstOrDefaultAsync();
        }

        public async Task<Clinic?> GetClinicByDoctorUserIdAsync(string userId)
        {
            return await _context.DoctorProfiles
                .Where(d => d.AppUserId == userId && d.IsApproved)
                .Select(d => d.Clinic)
                .FirstOrDefaultAsync();
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

    }

}
