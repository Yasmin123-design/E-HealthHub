using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.Helpers;
using Microsoft.EntityFrameworkCore;

namespace E_PharmaHub.Repositories.PharmacyRepo
{
    public class PharmacyRepository : IPharmacyRepository
    {
        private readonly EHealthDbContext _context;

        public PharmacyRepository(EHealthDbContext context)
        {
            _context = context;
        }
        private IQueryable<Pharmacy> BasePharmacyIncludes()
        {
            return _context.Pharmacies
                .Include(p => p.Address)
                .Include(p => p.Inventory)
                    .ThenInclude(i => i.Medication)
                .Include(p => p.Reviews)
                .AsNoTracking();
        }
       

        public async Task<Pharmacy?> GetPharmacyByPharmacistUserIdAsync(string userId)
        {
            return await _context.Pharmacists
                .Where(p => p.AppUserId == userId && p.IsApproved)
                .Select(p => p.Pharmacy)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Pharmacy>> GetAllAsync()
        {
            return await BasePharmacyIncludes()
                .Where(p => _context.Pharmacists.Any(ph => ph.PharmacyId == p.Id && ph.IsApproved))
                .ToListAsync();
        }

        public async Task<Pharmacy> GetByIdAsync(int id)
        {
            return await BasePharmacyIncludes()
                .Where(p => p.Id == id && _context.Pharmacists.Any(ph => ph.PharmacyId == p.Id && ph.IsApproved))
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(Pharmacy entity)
        {
            await _context.Pharmacies.AddAsync(entity);
        }

        public async Task Update(Pharmacy entity)
        {
            _context.Pharmacies.Update(entity);
        }

        public void Delete(Pharmacy entity)
        {
            _context.Pharmacies.Remove(entity);
        }

        public async Task<IEnumerable<PharmacySimpleDto>> GetAllBriefAsync()
        {
            return await BasePharmacyIncludes()
                .Where(p => _context.Pharmacists.Any(ph => ph.PharmacyId == p.Id && ph.IsApproved))
                .Select(PharmacySelectors.PharmacySimpleDtoSelector)
                .ToListAsync();
        }
        public async Task<IEnumerable<PharmacySimpleDto>> GetNearestPharmaciesWithMedicationAsync(
    string medicationName, double userLat, double userLng)
        {
            var pharmacies = await BasePharmacyIncludes()
                .Where(p => p.Inventory.Any(i =>
                    i.Medication.BrandName.Contains(medicationName) ||
                    i.Medication.GenericName.Contains(medicationName)))
                .Select(PharmacySelectors.PharmacySimpleDtoSelector)
                .ToListAsync();

            foreach (var p in pharmacies)
            {
                if (p.Latitude.HasValue && p.Longitude.HasValue)
                {
                    double dLat = (p.Latitude.Value - userLat) * Math.PI / 180;
                    double dLng = (p.Longitude.Value - userLng) * Math.PI / 180;
                    double lat1 = userLat * Math.PI / 180;
                    double lat2 = p.Latitude.Value * Math.PI / 180;

                    double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                               Math.Cos(lat1) * Math.Cos(lat2) *
                               Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
                    double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

                    double distance = 6371 * c;
                    p.DistanceFromUser = distance;
                }
                else
                {
                    p.DistanceFromUser = double.MaxValue;
                }
            }

            return pharmacies.OrderBy(p => p.DistanceFromUser).ToList();
        }

        public async Task<PharmacySimpleDto> GetByIdBriefAsync(int id)
        {
            return await BasePharmacyIncludes()
                .Where(p => p.Id == id && _context.Pharmacists.Any(ph => ph.PharmacyId == p.Id && ph.IsApproved))
                .Select(PharmacySelectors.PharmacySimpleDtoSelector)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PharmacySimpleDto>> GetTopRatedPharmaciesAsync(int count)
        {
            var pharmacies = await BasePharmacyIncludes()
                .Where(p => _context.Pharmacists.Any(ph => ph.PharmacyId == p.Id && ph.IsApproved))
                .OrderByDescending(p =>
                    p.Reviews.Any()
                        ? p.Reviews.Average(r => (double?)r.Rating)
                        : 0
                )
                .Take(count)
                .Select(PharmacySelectors.PharmacySimpleDtoSelector)
                .ToListAsync();

            return pharmacies;
        }

    }
}

