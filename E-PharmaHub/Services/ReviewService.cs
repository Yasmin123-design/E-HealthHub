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


        public async Task<IEnumerable<Review>> GetReviewsByPharmacyIdAsync(int pharmacyId)
        {
            return await _unitOfWork.Reviews.GetReviewsByPharmacyIdAsync(pharmacyId);
        }

        public async Task<IEnumerable<Review>> GetReviewsByMedicationIdAsync(int medicationId)
        {
            return await _unitOfWork.Reviews.GetReviewsByMedicationIdAsync(medicationId);
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
            existingReview.PharmacyId = updatedReview.PharmacyId;
            existingReview.MedicationId = updatedReview.MedicationId;

            _unitOfWork.Reviews.Update(existingReview);
            await _unitOfWork.CompleteAsync();

            return true;
        }
    }

}
