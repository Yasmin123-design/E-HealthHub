using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories.ClinicRepo
{
    public interface IClinicRepository : IGenericRepository<Clinic>
    {
        Task<IEnumerable<ClinicDto>> GetAllDtoAsync();
        Task<ClinicDto?> GetDtoByIdAsync(int id);
        Task<Clinic?> GetClinicByDoctorUserIdAsync(string userId);

    }
}
