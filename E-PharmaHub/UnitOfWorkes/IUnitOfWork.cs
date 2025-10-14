using E_PharmaHub.Models;
using E_PharmaHub.Repositories;

namespace E_PharmaHub.UnitOfWorkes
{
    public interface IUnitOfWork
    {
        IMedicineRepository Medicines { get; }
        IPharmacistRepository PharmasistsProfile { get; }
        IPharmacyRepository Pharmacies { get; }
        IUserRepository Useres { get; }
        IClinicRepository Clinics { get; }
        IDonorRepository Donors { get; }
        IReviewRepository Reviews { get; }
        IFavoriteMedicationRepository Favorite { get; }
        IFavouriteClinicRepository FavouriteClinic { get; }
        ICartRepository Carts { get; }
        IInventoryItemRepository IinventoryItem { get; }
        IOrderRepository Order { get; }
        IDoctorRepository Doctors { get; }
        IPaymentRepository Payments { get; }

        IBloodRequestRepository BloodRequest { get; }
        IAddressRepository Addresses { get; }
        IDonorMatchRepository donorMatches { get; }
        Task<int> CompleteAsync();
    }
}
