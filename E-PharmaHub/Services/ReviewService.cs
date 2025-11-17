using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.UnitOfWorkes;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<ReviewDto>> GetReviewsByDoctorIdAsync(int doctorId)
        {
            var reviews = await _unitOfWork.Reviews.GetReviewsByDoctorIdAsync(doctorId);

            return reviews.Select(r => new ReviewDto
            {
                Rating = r.Rating,
                Comment = r.Comment,
                UserEmail = r.User.Email
            });
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
        public async Task<IEnumerable<PharmacySimpleDto>> GetTopRatedPharmaciesAsync()
        {
            var pharmacies = await _unitOfWork.Reviews.GetTopRatedPharmaciesAsync(3);
            return pharmacies;
        
        }
        public async Task<IEnumerable<DoctorReadDto>> GetTopRatedDoctorsAsync()
        {
            var doctors = await _unitOfWork.Reviews.GetTopRatedDoctorsAsync(3);
            return doctors;
        }
        public async Task<IEnumerable<MedicineDto>> GetTopRatedMedicationsAsync()
        {
            var meds = await _unitOfWork.Reviews.GetTopRatedMedicationsAsync(3);
            return meds;
            
        }
    }

}
