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
        public async Task<IEnumerable<PharmacySimpleDto>> GetTopRatedPharmaciesAsync()
        {
            var pharmacies = await _unitOfWork.Reviews.GetTopRatedPharmaciesAsync(3);

            var dtoList = pharmacies.Select(p => new PharmacySimpleDto
            {
                Id = p.Id,
                Name = p.Name,
                Phone = p.Phone,
                City = p.Address.City,
                ImagePath = p.ImagePath,
                PostalCode = p.Address.PostalCode,
                Country = p.Address.Country,
                Street = p.Address.Street,
                Latitude = p.Address.Latitude,
                Longitude = p.Address.Longitude,
                AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0

            }).ToList();

            return dtoList;
        }
        public async Task<IEnumerable<DoctorReadDto>> GetTopRatedDoctorsAsync()
        {
            var doctors = await _unitOfWork.Reviews.GetTopRatedDoctorsAsync(3);

            var result = doctors.Select(d => new DoctorReadDto
            {
                Id = d.Id,
                Email = d.AppUser?.Email,
                Specialty = d.Specialty,
                IsApproved = d.IsApproved,
                Gender = d.Gender,
                ConsultationPrice = d.ConsultationPrice,
                ConsultationType = d.ConsultationType,
                ClinicName = d.Clinic.Name, 
                ClinicPhone = d.Clinic.Phone,
                ClinicImagePath = d.Clinic.ImagePath,
                DoctorImage = d.Image,
                City = d.Clinic.Address.City,
                Country = d.Clinic.Address.Country,
                Latitude = d.Clinic.Address.Latitude,
                Longitude = d.Clinic.Address.Longitude,
                Street = d.Clinic.Address.Street,
                PostalCode = d.Clinic.Address.PostalCode,
                Username = d.AppUser?.UserName,
                AverageRating  = d.Reviews.Any() ? d.Reviews.Average(r => r.Rating) : 0,

            });

            return result;
        }
        public async Task<IEnumerable<MedicineDto>> GetTopRatedMedicationsAsync()
        {
            var meds = await _unitOfWork.Reviews.GetTopRatedMedicationsAsync(3);
            return meds;
            
        }
    }

}
