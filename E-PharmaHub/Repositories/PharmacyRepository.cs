using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace E_PharmaHub.Repositories
{
    public class PharmacyRepository : IPharmacyRepository
    {
        private readonly EHealthDbContext _context;

        public PharmacyRepository(EHealthDbContext context)
        {
            _context = context;
        }
        public async Task<Pharmacy?> GetPharmacyByPharmacistUserIdAsync(string userId)
        {
            return await _context.Pharmacists
                .Where(p => p.AppUserId == userId)
                .Select(p => p.Pharmacy)
                .FirstOrDefaultAsync();
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

        public async Task<IEnumerable<PharmacySimpleDto>> GetAllBriefAsync()
        {
            var pharmacies = await _context.Pharmacies
                    .Include(p => p.Address)
                    .Select(p => new PharmacySimpleDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Phone = p.Phone,
                        City = p.Address.City,
                        ImagePath = p.ImagePath
                    })
                    .ToListAsync();
            return pharmacies;
        }

        public async Task<PharmacySimpleDto> GetByIdBriefAsync(int id)
        {
            return  _context.Pharmacies
                    .Include(p => p.Address)
                    .Select(p => new PharmacySimpleDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Phone = p.Phone,
                        City = p.Address.City,
                        ImagePath = p.ImagePath
                    })
                    .FirstOrDefault(x => x.Id == id);
        }
    }
}
