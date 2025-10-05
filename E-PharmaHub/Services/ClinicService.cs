using E_PharmaHub.Models;
using E_PharmaHub.UnitOfWorkes;

namespace E_PharmaHub.Services
{
    public class ClinicService : IClinicService
    {
        private readonly IUnitOfWork _unitOfWork;


        public ClinicService(IUnitOfWork unitOfWork,IFileStorageService fileStorage)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Clinic> CreateClinicAsync(Clinic clinic)
        {
            await _unitOfWork.Clinics.AddAsync(clinic);
            await _unitOfWork.CompleteAsync();
            return clinic;
        }

        public async Task<Clinic?> GetClinicByIdAsync(int id)
        {
            return await _unitOfWork.Clinics.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Clinic>> GetAllClinicsAsync()
        {
            return await _unitOfWork.Clinics.GetAllAsync();
        }


        public async Task<bool> UpdateClinicAsync(Clinic clinic)
        {
            _unitOfWork.Clinics.Update(clinic);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> DeleteClinicAsync(int id)
        {
            var clinic = await _unitOfWork.Clinics.GetByIdAsync(id);
            if (clinic == null) return false;

            _unitOfWork.Clinics.Delete(clinic);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }

}
