using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using System.Linq.Expressions;

namespace E_PharmaHub.Repositories
{
    public interface IMedicineRepository : IGenericRepository<Medication>
    {
        Task<Medication?> FindAsync(Expression<Func<Medication, bool>> predicate);
        Task<IEnumerable<Medication>> SearchByNameAsync(string name);
        Task<IEnumerable<PharmacySimpleDto>> GetNearestPharmaciesWithMedicationAsync(string medicationName, double userLat, double userLng);
            Task<IEnumerable<Medication>> GetMedicinesByPharmacyIdAsync(int pharmacyId);


    }
}
