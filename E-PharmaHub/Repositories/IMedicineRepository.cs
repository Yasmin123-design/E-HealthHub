using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using System.Linq.Expressions;

namespace E_PharmaHub.Repositories
{
    public interface IMedicineRepository 
    {
        Task<Medication?> FindAsync(Expression<Func<Medication, bool>> predicate);
        Task<Medication> GetByIdAsync(int id);
        Task AddAsync(Medication entity);
        Task Update(Medication entity);
        void Delete(Medication entity);
        Task<IEnumerable<Medication>> GetAllAsync();
        Task<IEnumerable<MedicineDto>> SearchByNameAsync(string name);
        Task<IEnumerable<MedicineDto>> GetMedicinesByPharmacyIdAsync(int pharmacyId);
        Task<IEnumerable<MedicineDto>> GetTopRatedMedicationsAsync(int count);


    }
}
