﻿using E_PharmaHub.Models;

namespace E_PharmaHub.Services
{
    public interface IReviewService
    {
        Task<IEnumerable<Review>> GetAllReviewsAsync();
        Task<Review> GetReviewByIdAsync(int id);
        Task AddReviewAsync(Review review);
        Task<bool> UpdateReviewAsync(int id, Review updatedReview, string userId);
        Task<bool> DeleteReviewAsync(int id, string userId);

        Task<IEnumerable<Review>> GetReviewsByPharmacyIdAsync(int pharmacyId);
        Task<IEnumerable<Review>> GetReviewsByMedicationIdAsync(int medicationId);
        Task<double> GetAverageRatingForPharmacyAsync(int pharmacyId);
        Task<double> GetAverageRatingForMedicationAsync(int medicationId);

    }
}
