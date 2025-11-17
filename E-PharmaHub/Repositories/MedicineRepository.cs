using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace E_PharmaHub.Repositories
{
    public class MedicineRepository : IMedicineRepository
    {
        private readonly EHealthDbContext _context;

        public MedicineRepository(EHealthDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Medication>> GetAllAsync()
        {
            return await _context.Medications
                .AsNoTracking()
                .Include(m => m.Inventories)
                    .ThenInclude(i => i.Pharmacy)
                        .ThenInclude(p => p.Address)
                .ToListAsync();
        }

        public async Task<Medication?> FindAsync(Expression<Func<Medication, bool>> predicate)
        {
            return await _context.Medications
                .AsNoTracking()
                .Include(m => m.Inventories)
                    .ThenInclude(i => i.Pharmacy)
                        .ThenInclude(a => a.Address)
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<Medication> GetByIdAsync(int id)
        {
            return await _context.Medications
                .AsNoTracking()
                .Include(m => m.Inventories)
                    .ThenInclude(i => i.Pharmacy)
                        .ThenInclude(a => a.Address)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
        public async Task AddAsync(Medication entity)
        {
            await _context.Medications.AddAsync(entity);
        }

        public async Task Update(Medication entity)
        {
            _context.Medications.Update(entity);
        }

        public void Delete(Medication entity)
        {
            _context.Medications.Remove(entity);
        }
        public async Task<IEnumerable<Medication>> SearchByNameAsync(string name)
        {
            return await _context.Medications
                .AsNoTracking()
                .Include(m => m.Reviews)
                .Include(m => m.Inventories)
                    .ThenInclude(i => i.Pharmacy)
                        .ThenInclude(a => a.Address)
                .Where(m => m.BrandName.Contains(name) ||
                            m.GenericName.Contains(name))
                .ToListAsync();
        }

        public async Task<IEnumerable<Medication>> GetMedicinesByPharmacyIdAsync(int pharmacyId)
        {
            return await _context.Medications
                .AsNoTracking()
                .Include(m => m.Reviews)
                .Include(m => m.Inventories.Where(inv => inv.PharmacyId == pharmacyId)) // Filter قبل Load
                    .ThenInclude(inv => inv.Pharmacy)
                        .ThenInclude(p => p.Address)
                .Where(m => m.Inventories.Any(inv => inv.PharmacyId == pharmacyId))
                .ToListAsync();
        }
        public async Task<IEnumerable<PharmacySimpleDto>> GetNearestPharmaciesWithMedicationAsync(
            string medicationName, double userLat, double userLng)
        {
            var pharmacies = await _context.Pharmacies
    .AsNoTracking()
    .Where(p => p.Inventory.Any(i =>
        i.Medication.BrandName.Contains(medicationName) ||
        i.Medication.GenericName.Contains(medicationName)))
    .Select(p => new PharmacySimpleDto
    {
        Id = p.Id,
        Name = p.Name,
        Phone = p.Phone,
        ImagePath = p.ImagePath,
        City = p.Address.City,
        Street = p.Address.Street,
        PostalCode = p.Address.PostalCode,
        Country = p.Address.Country,
        Latitude = p.Address.Latitude,
        Longitude = p.Address.Longitude,
        AverageRating = p.Inventory
            .SelectMany(i => i.Medication.Reviews)
            .Any() ? p.Inventory.SelectMany(i => i.Medication.Reviews).Average(r => r.Rating) : 0
    })
    .ToListAsync();

            foreach (var p in pharmacies)
            {
                if (p.Latitude.HasValue && p.Longitude.HasValue)
                {
                    var dLat = (p.Latitude.Value - userLat) * Math.PI / 180;
                    var dLng = (p.Longitude.Value - userLng) * Math.PI / 180;
                    var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                            Math.Cos(userLat * Math.PI / 180) *
                            Math.Cos(p.Latitude.Value * Math.PI / 180) *
                            Math.Sin(dLng / 2) * Math.Sin(dLng / 2);

                    var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                }
            }

            return pharmacies.ToList();
        }
    }
}

