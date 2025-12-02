using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.UnitOfWorkes;

namespace E_PharmaHub.Services.ReviewServ
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

        public async Task<Review?> GetReviewByIdAsync(int id)
        {
            return await _unitOfWork.Reviews.GetByIdAsync(id);
        }

        public async Task AddReviewAsync(Review review)
        {
            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> UpdateReviewAsync(int id, Review updatedReview, string userId)
        {
            var existingReview = await _unitOfWork.Reviews.GetByIdAsync(id);
            if (existingReview == null || existingReview.UserId != userId)
                return false;

            existingReview.Rating = updatedReview.Rating;
            existingReview.Comment = updatedReview.Comment;

            _unitOfWork.Reviews.Update(existingReview);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> DeleteReviewAsync(int id, string userId)
        {
            var existingReview = await _unitOfWork.Reviews.GetByIdAsync(id);
            if (existingReview == null || existingReview.UserId != userId)
                return false;

            _unitOfWork.Reviews.Delete(existingReview);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByPharmacyIdAsync(int pharmacyId)
        {
            return await _unitOfWork.Reviews.GetReviewDtosByPharmacyIdAsync(pharmacyId);
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByMedicationIdAsync(int medicationId)
        {
            return await _unitOfWork.Reviews.GetReviewDtosByMedicationIdAsync(medicationId);
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByDoctorIdAsync(int doctorId)
        {
            return await _unitOfWork.Reviews.GetReviewDtosByDoctorIdAsync(doctorId);
        }

    }

}
