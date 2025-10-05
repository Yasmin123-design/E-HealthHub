using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace E_PharmaHub.Repositories
{
    public class PharmacyRepository : IGenericRepository<Pharmacy>
    {
        private readonly EHealthDbContext _context;

        public PharmacyRepository(EHealthDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Pharmacy>> GetAllAsync()
        {
            return await _context.Pharmacies
                .Include(p => p.Address)
                .Include(p => p.Inventory)
                    .ThenInclude(i => i.Medication)
                .ToListAsync();
        }

        public async Task<Pharmacy> GetByIdAsync(int id)
        {
            return await _context.Pharmacies
                .Include(p => p.Address)
                .Include(p => p.Inventory)
                    .ThenInclude(i => i.Medication)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Pharmacy entity)
        {
            await _context.Pharmacies.AddAsync(entity);
        }

        public void Update(Pharmacy entity)
        {
            _context.Pharmacies.Update(entity);
        }

        public void Delete(Pharmacy entity)
        {
            _context.Pharmacies.Remove(entity);
        }

    }
}
