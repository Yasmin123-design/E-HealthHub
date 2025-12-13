using E_PharmaHub.Dtos;
using E_PharmaHub.Helpers;
using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace E_PharmaHub.Repositories.ReviewRepo
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly EHealthDbContext _context;

        public ReviewRepository(EHealthDbContext context)
        {
            _context = context;
        }
        private IQueryable<Review> BaseReviewInclude()
        {
            return _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Pharmacy)
                .Include(r => r.Medication);
        }

        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            return await BaseReviewInclude().ToListAsync();
        }

        public async Task<Review?> GetByIdAsync(int id)
        {
            return await BaseReviewInclude()
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
            return await BaseReviewInclude()
                .Where(r => r.PharmacyId == pharmacyId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetReviewsByMedicationIdAsync(int medicationId)
        {
            return await BaseReviewInclude()
                .Where(r => r.MedicationId == medicationId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetReviewsByDoctorIdAsync(int doctorId)
        {
            return await BaseReviewInclude()
                .Where(r => r.DoctorId == doctorId)
                .ToListAsync();
        }
        public async Task<IEnumerable<ReviewDto>> GetReviewDtosByPharmacyIdAsync(int pharmacyId)
        {
            return await BaseReviewInclude()
                .Where(r => r.PharmacyId == pharmacyId)
                .Select(ReviewSelectors.ReviewDtoSelector)
                .ToListAsync();
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewDtosByMedicationIdAsync(int medicationId)
        {
            return await BaseReviewInclude()
                .Where(r => r.MedicationId == medicationId)
                .Select(ReviewSelectors.ReviewDtoSelector)
                .ToListAsync();
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewDtosByDoctorIdAsync(int doctorId)
        {
            return await BaseReviewInclude()
                .Where(r => r.DoctorId == doctorId)
                .Select(ReviewSelectors.ReviewDtoSelector)
                .ToListAsync();
        }
        public async Task<int> GetReviewsCountAsync(int doctorId)
        {
            return await _context.Reviews
                .CountAsync(r => r.DoctorId == doctorId);
        }
    }

}
