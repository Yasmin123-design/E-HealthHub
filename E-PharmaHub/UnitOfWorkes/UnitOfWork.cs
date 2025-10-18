using E_PharmaHub.Models;
using E_PharmaHub.Repositories;


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

        public IClinicRepository Clinics { get; }

        public IUserRepository Useres { get; }
        public IChatRepository Chat { get; private set; }
        public IMessageThreadRepository MessageThread { get; private set; }

        public IAppointmentRepository Appointments { get; private set; }

        public IPrescriptionRepository Prescriptions { get; private set; }

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
            IPrescriptionRepository prescriptionRepository
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
            Chat = chatRepository;
            MessageThread = messageThreadRepository;
            Appointments = appointmentRepository;
            Prescriptions = prescriptionRepository;
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
