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
        public IGenericRepository<Clinic> Clinics { get; }
        public IDonorRepository Donors { get; }
        public IBloodRequestRepository BloodRequest { get; }
        public IInventoryItemRepository IinventoryItem { get; }
        public ICartRepository Carts { get; }
        public IPharmacistRepository PharmasistsProfile { get; }
        public IDonorMatchRepository donorMatches { get; }
        public IPaymentRepository Payments { get; }

        public IOrderRepository Order  { get; }

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
            IOrderRepository orderRepository
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
