using E_PharmaHub.Models;
using E_PharmaHub.Repositories;
using E_PharmaHub.Repositories.AddressRepo;
using E_PharmaHub.Repositories.AppointmentRepo;
using E_PharmaHub.Repositories.CartRepo;
using E_PharmaHub.Repositories.ChatRepo;
using E_PharmaHub.Repositories.ClinicRepo;
using E_PharmaHub.Repositories.DoctorRepo;
using E_PharmaHub.Repositories.FavoriteMedicationRepo;
using E_PharmaHub.Repositories.FavouriteClinicRepo;
using E_PharmaHub.Repositories.FavouriteDoctorRepo;
using E_PharmaHub.Repositories.MedicineRepo;
using E_PharmaHub.Repositories.PharmacyRepo;
using E_PharmaHub.Repositories.PrescriptionRepo;
using E_PharmaHub.Repositories.OrderRepo;
using E_PharmaHub.Repositories.PaymentRepo;
using E_PharmaHub.Repositories.PharmacistRepo;
using E_PharmaHub.Repositories.ReviewRepo;
using E_PharmaHub.Repositories.UserRepo;
using E_PharmaHub.Repositories.InventoryItemRepo;
using E_PharmaHub.Repositories.MessageThreadRepo;
using E_PharmaHub.Repositories.NotificationRepo;

namespace E_PharmaHub.UnitOfWorkes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EHealthDbContext _context;
        public IMedicineRepository Medicines { get; }
        public IReviewRepository Reviews { get; }
        public IDoctorRepository Doctors { get; }
        public IPharmacyRepository Pharmacies { get; private set; }
        public IAddressRepository Addresses { get; private set; }
        public IDonorRepository Donors { get; }
        public IBloodRequestRepository BloodRequest { get; }
        public IInventoryItemRepository IinventoryItem { get; }
        public ICartRepository Carts { get; }
        public IPharmacistRepository PharmasistsProfile { get; }
        public IDonorMatchRepository donorMatches { get; }
        public IPaymentRepository Payments { get; }
        public IOrderRepository Order  { get; }

        public IFavoriteMedicationRepository Favorite { get; }

        public IFavouriteClinicRepository FavouriteClinic { get; }
        public IFavouriteDoctorRepository FavouriteDoctor { get; }
        public IClinicRepository Clinics { get; }

        public IUserRepository Useres { get; }
        public IChatRepository Chat { get; private set; }
        public IMessageThreadRepository MessageThread { get; private set; }

        public IAppointmentRepository Appointments { get; private set; }

        public IPrescriptionRepository Prescriptions { get; private set; }
        public INotificationRepository Notifications { get; private set; }

        public UnitOfWork(EHealthDbContext context,
            IMedicineRepository medicineRepository,
            IReviewRepository reviewRepository,
            IDoctorRepository doctorRepository,
            IInventoryItemRepository inventoryItemRepository,
            IPharmacistRepository pharmacistRepository,
            IAddressRepository addressRepository,
            IBloodRequestRepository bloodRequestRepository,
            IDonorRepository donorRepository,
            IDonorMatchRepository donorMatchRepository,
            IPaymentRepository paymentRepository,
            IPharmacyRepository pharmacyRepository,
            ICartRepository cartRepository ,
            IOrderRepository orderRepository,
            IFavoriteMedicationRepository favoriteMedicationRepository,
            IFavouriteClinicRepository favouriteClinicRepository,
            IClinicRepository clinicRepository,
            IUserRepository userRepository,
            IChatRepository chatRepository,
            IMessageThreadRepository messageThreadRepository,
            IAppointmentRepository appointmentRepository,
            IPrescriptionRepository prescriptionRepository,
            IFavouriteDoctorRepository favouriteDoctorRepository,
            INotificationRepository notificationRepository
            )
        {
            _context = context;
            Payments = paymentRepository;
            Medicines = medicineRepository;
            IinventoryItem = inventoryItemRepository;
            Pharmacies = pharmacyRepository;
            Reviews = reviewRepository;
            Carts = cartRepository;
            BloodRequest = bloodRequestRepository;
            Addresses = addressRepository;
            PharmasistsProfile = pharmacistRepository;
            Doctors = doctorRepository;
            Clinics = new ClinicRepository(_context);
            Donors = donorRepository;
            donorMatches = donorMatchRepository;
            Order = orderRepository;
            Favorite = favoriteMedicationRepository;
            FavouriteClinic = favouriteClinicRepository;
            Clinics = clinicRepository;
            Useres = userRepository;
            Notifications = notificationRepository;
            Chat = chatRepository;
            MessageThread = messageThreadRepository;
            Appointments = appointmentRepository;
            Prescriptions = prescriptionRepository;
            FavouriteDoctor = favouriteDoctorRepository;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

}
