using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace E_PharmaHub.Repositories
{
    public class PharmacistRepository : IGenericRepository<PharmacistProfile>
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
    }
}
