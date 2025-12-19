using E_PharmaHub.Repositories;
using E_PharmaHub.Repositories.AddressRepo;
using E_PharmaHub.Repositories.AppointmentRepo;
using E_PharmaHub.Repositories.CartItemRepo;
using E_PharmaHub.Repositories.CartRepo;
using E_PharmaHub.Repositories.ChatRepo;
using E_PharmaHub.Repositories.ClinicRepo;
using E_PharmaHub.Repositories.DoctorRepo;
using E_PharmaHub.Repositories.FavoriteMedicationRepo;
using E_PharmaHub.Repositories.FavouriteClinicRepo;
using E_PharmaHub.Repositories.FavouriteDoctorRepo;
using E_PharmaHub.Repositories.InventoryItemRepo;
using E_PharmaHub.Repositories.MedicineRepo;
using E_PharmaHub.Repositories.MessageThreadRepo;
using E_PharmaHub.Repositories.NotificationRepo;
using E_PharmaHub.Repositories.OrderRepo;
using E_PharmaHub.Repositories.PaymentRepo;
using E_PharmaHub.Repositories.PharmacistRepo;
using E_PharmaHub.Repositories.PharmacyRepo;
using E_PharmaHub.Repositories.PrescriptionItemRepo;
using E_PharmaHub.Repositories.PrescriptionRepo;
using E_PharmaHub.Repositories.ReviewRepo;
using E_PharmaHub.Repositories.UserRepo;

namespace E_PharmaHub.UnitOfWorkes
{
    public interface IUnitOfWork
    {
        IMedicineRepository Medicines { get; }
        IPharmacistRepository PharmasistsProfile { get; }
        IAppointmentRepository Appointments { get; }
        IPharmacyRepository Pharmacies { get; }

        IUserRepository Useres { get; }
        IPrescriptionRepository Prescriptions { get; }
        IClinicRepository Clinics { get; }
        IDonorRepository Donors { get; }
        IChatRepository Chat { get; }
        IMessageThreadRepository MessageThread { get; }
        IReviewRepository Reviews { get; }
        IFavoriteMedicationRepository Favorite { get; }
        IFavouriteClinicRepository FavouriteClinic { get; }
        IFavouriteDoctorRepository FavouriteDoctor { get; }
        ICartRepository Carts { get; }
        IInventoryItemRepository IinventoryItem { get; }
        IOrderRepository Order { get; }
        IDoctorRepository Doctors { get; }
        IPaymentRepository Payments { get; }

        IBloodRequestRepository BloodRequest { get; }
        IAddressRepository Addresses { get; }
        IDonorMatchRepository donorMatches { get; }
        INotificationRepository Notifications { get; }
        ICartItemRepository CartItemRepository { get; }
        IPrescriptionItemRepository PrescriptionItems { get; }


        Task<int> CompleteAsync();
    }
}
