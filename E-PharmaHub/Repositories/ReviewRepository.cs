using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace E_PharmaHub.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly EHealthDbContext _context;

        public ReviewRepository(EHealthDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Pharmacy)
                .Include(r => r.Medication)
                .ToListAsync();
        }

        public async Task<Review> GetByIdAsync(int id)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Pharmacy)
                .Include(r => r.Medication)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddAsync(Review entity)
        {
            await _context.Reviews.AddAsync(entity);
        }

        public async Task Update(Review entity)
        {
            _context.Reviews.Update(entity);
        }

        public void Delete(Review entity)
        {
            _context.Reviews.Remove(entity);
        }

        public async Task<IEnumerable<Review>> GetReviewsByPharmacyIdAsync(int pharmacyId)
        {
            return await _context.Reviews
                .Where(r => r.PharmacyId == pharmacyId)
                .Include(r => r.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetReviewsByMedicationIdAsync(int medicationId)
        {
            return await _context.Reviews
                .Where(r => r.MedicationId == medicationId)
                .Include(r => r.User)
                .ToListAsync();
        }

        public async Task<double> GetAverageRatingForPharmacyAsync(int pharmacyId)
        {
            return await _context.Reviews
                .Where(r => r.PharmacyId == pharmacyId)
                .AverageAsync(r => (double?)r.Rating) ?? 0;
        }

        public async Task<double> GetAverageRatingForMedicationAsync(int medicationId)
        {
            return await _context.Reviews
                .Where(r => r.MedicationId == medicationId)
                .AverageAsync(r => (double?)r.Rating) ?? 0;
        }
        public async Task<IEnumerable<Pharmacy>> GetTopRatedPharmaciesAsync(int count)
        {
            return await _context.Pharmacies
                .Include(p => p.Reviews)
                .OrderByDescending(p => p.Reviews.Average(r => (double?)r.Rating) ?? 0)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<DoctorProfile>> GetTopRatedDoctorsAsync(int count)
        {
            return await _context.DoctorProfiles
                .Include(d => d.Reviews)
                .Include(d => d.AppUser)
                .OrderByDescending(d => d.Reviews.Average(r => (double?)r.Rating) ?? 0)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Medication>> GetTopRatedMedicationsAsync(int count)
        {
            return await _context.Medications
                .Include(m => m.Reviews)
                .OrderByDescending(m => m.Reviews.Average(r => (double?)r.Rating) ?? 0)
                .Take(count)
                .ToListAsync();
        }
    }

}
