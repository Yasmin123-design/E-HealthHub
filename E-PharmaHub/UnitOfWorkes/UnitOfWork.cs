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
        public IGenericRepository<Pharmacy> Pharmacies { get; private set; }
        public IGenericRepository<Address> Addresses { get; private set; }
        public IGenericRepository<Clinic> Clinics { get; }
        public IGenericRepository<DonorProfile> Donors { get; }

        public IInventoryItemRepository IinventoryItem { get; }

        public IGenericRepository<PharmacistProfile> PharmasistsProfile { get; }

        public UnitOfWork(EHealthDbContext context,
            IMedicineRepository medicineRepository,
            IReviewRepository reviewRepository,
            IDoctorRepository doctorRepository,
            IInventoryItemRepository inventoryItemRepository
            )
        {
            _context = context;
            Medicines = medicineRepository;
            IinventoryItem = inventoryItemRepository;
            Pharmacies = new PharmacyRepository(_context);
            Reviews = reviewRepository;
            Addresses = new AddressRepository(_context);
            PharmasistsProfile = new PharmacistRepository(_context);
            Doctors = doctorRepository;
            Clinics = new ClinicRepository(_context);
            Donors = new DonorRepository(_context);
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
