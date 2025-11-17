using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<IEnumerable<Review>> GetReviewsByPharmacyIdAsync(int pharmacyId);
        Task<IEnumerable<Review>> GetReviewsByMedicationIdAsync(int medicationId);
        Task<IEnumerable<Review>> GetReviewsByDoctorIdAsync(int doctorId);
        Task<IEnumerable<PharmacySimpleDto>> GetTopRatedPharmaciesAsync(int count);
        Task<IEnumerable<DoctorReadDto>> GetTopRatedDoctorsAsync(int count);
        Task<IEnumerable<MedicineDto>> GetTopRatedMedicationsAsync(int count);

    }

}
