using E_PharmaHub.Models;
using E_PharmaHub.Repositories;

namespace E_PharmaHub.UnitOfWorkes
{
    public interface IUnitOfWork
    {
        IMedicineRepository Medicines { get; }
        IPharmacistRepository PharmasistsProfile { get; }
        IGenericRepository<Pharmacy> Pharmacies { get; }
        IGenericRepository<Clinic> Clinics { get; }
        IGenericRepository<DonorProfile> Donors { get; }
        IReviewRepository Reviews { get; }
        IInventoryItemRepository IinventoryItem { get; }
        IDoctorRepository Doctors { get; }
        IAddressRepository Addresses { get; }
        Task<int> CompleteAsync();
    }
}
