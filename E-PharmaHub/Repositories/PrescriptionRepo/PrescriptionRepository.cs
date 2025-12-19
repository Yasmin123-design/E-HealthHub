using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace E_PharmaHub.Repositories.PrescriptionRepo
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
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        

        public async Task<List<Prescription>> GetByUserIdAsync(string userId)
        {
            return await _context.Prescriptions
                .Include(p => p.Items)
                .Include(p => p.Doctor)
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task<Prescription?> GetWithItemsAsync(int id)
        {
            return await _context.Prescriptions
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Prescription prescription)
        {
            await _context.Prescriptions.AddAsync(prescription);
        }

        public async Task DeleteAsync(Prescription prescription)
        {
            var items = _context.PrescriptionItems
                .Where(i => i.PrescriptionId == prescription.Id);

            _context.PrescriptionItems.RemoveRange(items);

            _context.Prescriptions.Remove(prescription);

        }


    }
}
