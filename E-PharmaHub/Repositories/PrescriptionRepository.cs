using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace E_PharmaHub.Repositories
{
    public class PrescriptionRepository : IPrescriptionRepository
    {
        private readonly EHealthDbContext _context;

        public PrescriptionRepository(EHealthDbContext context)
        {
            _context = context;
        }

        public async Task<Prescription> GetByIdAsync(int id)
        {
            return await _context.Prescriptions
                .Include(u => u.User)
                .Include(p => p.Doctor)
                .ThenInclude(a => a.AppUser)
                .Include(p => p.Items)
                .ThenInclude(i => i.Medication)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Prescription>> GetByUserIdAsync(string userId)
        {
            return await _context.Prescriptions
                .Include(u => u.User)
                .Include(p => p.Doctor)
                .ThenInclude(a => a.AppUser)
                .Include(p => p.Items)
                .ThenInclude(i => i.Medication)
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prescription>> GetByDoctorIdAsync(int doctorId)
        {
            return await _context.Prescriptions
                 .Include(p => p.User)
                .Include(p => p.Doctor)
                .ThenInclude(a => a.AppUser)
                .Include(p => p.Items)
                .ThenInclude(i => i.Medication)
                .Where(p => p.DoctorId == doctorId)
                .ToListAsync();
        }

        public async Task AddAsync(Prescription prescription)
        {
            await _context.Prescriptions.AddAsync(prescription);
        }

        public async Task DeleteAsync(Prescription prescription)
        {
            _context.Prescriptions.Remove(prescription);
            await Task.CompletedTask;
        }

    }
}
