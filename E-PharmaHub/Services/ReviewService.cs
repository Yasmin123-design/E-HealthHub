using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.UnitOfWorkes;

namespace E_PharmaHub.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Review>> GetAllReviewsAsync()
        {
            return await _unitOfWork.Reviews.GetAllAsync();
        }

        public async Task<Review> GetReviewByIdAsync(int id)
        {
            return await _unitOfWork.Reviews.GetByIdAsync(id);
        }

        public async Task AddReviewAsync(Review review)
        {
            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.CompleteAsync();
        }


        public async Task<bool> DeleteReviewAsync(int id, string userId)
        {
            var existingReview = await _unitOfWork.Reviews.GetByIdAsync(id);

            if (existingReview == null)
                return false; 

            if (existingReview.UserId != userId)
                return false;

            _unitOfWork.Reviews.Delete(existingReview);
            await _unitOfWork.CompleteAsync();

            return true;
        }


        public async Task<IEnumerable<ReviewDto>> GetReviewsByPharmacyIdAsync(int pharmacyId)
        {
            var reviews = await _unitOfWork.Reviews.GetReviewsByPharmacyIdAsync(pharmacyId);

            return reviews.Select(r => new ReviewDto
            {
                Rating = r.Rating,
                Comment = r.Comment,
                UserEmail = r.User.Email
            });
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByMedicationIdAsync(int medicationId)
        {
            var reviews = await _unitOfWork.Reviews.GetReviewsByMedicationIdAsync(medicationId);

            return reviews.Select(r => new ReviewDto
            {
                Rating = r.Rating,
                Comment = r.Comment,
                UserEmail = r.User.Email
            });
        }



        public async Task<double> GetAverageRatingForPharmacyAsync(int pharmacyId)
        {
            return await _unitOfWork.Reviews.GetAverageRatingForPharmacyAsync(pharmacyId);
        }

        public async Task<double> GetAverageRatingForMedicationAsync(int medicationId)
        {
            return await _unitOfWork.Reviews.GetAverageRatingForMedicationAsync(medicationId);
        }

        public async Task<bool> UpdateReviewAsync(int id, Review updatedReview, string userId)
        {
            var existingReview = await _unitOfWork.Reviews.GetByIdAsync(id);

            if (existingReview == null)
                return false; 

            if (existingReview.UserId != userId)
                return false;

            existingReview.Rating = updatedReview.Rating;
            existingReview.Comment = updatedReview.Comment;

            _unitOfWork.Reviews.Update(existingReview);
            await _unitOfWork.CompleteAsync();

            return true;
        }
        public async Task<IEnumerable<Pharmacy>> GetTopRatedPharmaciesAsync(int count)
        {
            return await _unitOfWork.Reviews.GetTopRatedPharmaciesAsync(count);
        }
        public async Task<IEnumerable<object>> GetTopRatedPharmaciesAsync()
        {
            var pharmacies = await _unitOfWork.Reviews.GetTopRatedPharmaciesAsync(3);
            return pharmacies.Select(p => new
            {
                p.Id,
                p.Name,
                p.ImagePath,
                AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0
            });
        }

        public async Task<IEnumerable<object>> GetTopRatedDoctorsAsync()
        {
            var doctors = await _unitOfWork.Reviews.GetTopRatedDoctorsAsync(3);
            return doctors.Select(d => new
            {
                d.Id,
                DoctorName = d.AppUser?.UserName,
                d.Specialty,
                d.Image,
                AverageRating = d.Reviews.Any() ? d.Reviews.Average(r => r.Rating) : 0
            });
        }

        public async Task<IEnumerable<object>> GetTopRatedMedicationsAsync()
        {
            var meds = await _unitOfWork.Reviews.GetTopRatedMedicationsAsync(3);
            return meds.Select(m => new
            {
                m.Id,
                m.BrandName,
                m.GenericName,
                m.ImagePath,
                AverageRating = m.Reviews.Any() ? m.Reviews.Average(r => r.Rating) : 0
            });
        }
    }

}
