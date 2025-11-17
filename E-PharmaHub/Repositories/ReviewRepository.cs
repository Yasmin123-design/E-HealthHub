using E_PharmaHub.Dtos;
using E_PharmaHub.Helpers;
using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace E_PharmaHub.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly EHealthDbContext _context;

        public ReviewRepository(EHealthDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Pharmacy)
                .Include(r => r.Medication)
                .ToListAsync();
        }

        public async Task<Review> GetByIdAsync(int id)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Pharmacy)
                .Include(r => r.Medication)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddAsync(Review entity)
        {
            await _context.Reviews.AddAsync(entity);
        }

        public async Task Update(Review entity)
        {
            _context.Reviews.Update(entity);
        }

        public void Delete(Review entity)
        {
            _context.Reviews.Remove(entity);
        }

        public async Task<IEnumerable<Review>> GetReviewsByPharmacyIdAsync(int pharmacyId)
        {
            return await _context.Reviews
                .Where(r => r.PharmacyId == pharmacyId)
                .Include(r => r.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetReviewsByMedicationIdAsync(int medicationId)
        {
            return await _context.Reviews
                .Where(r => r.MedicationId == medicationId)
                .Include(r => r.User)
                .ToListAsync();
        }
        public async Task<IEnumerable<Review>> GetReviewsByDoctorIdAsync(int doctorId)
        {
            return await _context.Reviews
                .Where(r => r.DoctorId == doctorId)
                .Include(r => r.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<PharmacySimpleDto>> GetTopRatedPharmaciesAsync(int count)
        {
            var pharmacies =  await _context.Pharmacies
                .Include(a => a.Address)
                .Include(p => p.Reviews)
                .OrderByDescending(p => p.Reviews.Average(r => (double?)r.Rating) ?? 0)
                .Take(count)
                .ToListAsync();

            var dtoList = pharmacies.Select(p => new PharmacySimpleDto
            {
                Id = p.Id,
                Name = p.Name,
                Phone = p.Phone,
                City = p.Address.City,
                ImagePath = p.ImagePath,
                PostalCode = p.Address.PostalCode,
                Country = p.Address.Country,
                Street = p.Address.Street,
                Latitude = p.Address.Latitude,
                Longitude = p.Address.Longitude,
                AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0

            }).ToList();
            return dtoList;
        }

        public async Task<IEnumerable<DoctorReadDto>> GetTopRatedDoctorsAsync(int count)
        {
            var doctors =  await _context.DoctorProfiles
                .Include(d => d.Reviews)
                .Include(d => d.AppUser)
                .Include(d => d.Clinic) 
                .ThenInclude(a => a.Address)
                .OrderByDescending(d => d.Reviews.Average(r => (double?)r.Rating) ?? 0)
                .Take(count)
                .ToListAsync();
            var result = doctors.Select(d => new DoctorReadDto
            {
                Id = d.Id,
                Email = d.AppUser?.Email,
                Specialty = d.Specialty,
                IsApproved = d.IsApproved,
                Gender = d.Gender,
                ConsultationPrice = d.ConsultationPrice,
                ConsultationType = d.ConsultationType,
                ClinicName = d.Clinic.Name,
                ClinicPhone = d.Clinic.Phone,
                ClinicImagePath = d.Clinic.ImagePath,
                DoctorImage = d.Image,
                City = d.Clinic.Address.City,
                Country = d.Clinic.Address.Country,
                Latitude = d.Clinic.Address.Latitude,
                Longitude = d.Clinic.Address.Longitude,
                Street = d.Clinic.Address.Street,
                PostalCode = d.Clinic.Address.PostalCode,
                Username = d.AppUser?.UserName,
                AverageRating = d.Reviews.Any() ? d.Reviews.Average(r => r.Rating) : 0,
                CountPatient = _context.Appointments.Count(a => a.DoctorId == d.AppUserId),
                CountFavourite = _context.FavouriteDoctors.Count(a => a.DoctorId == d.Id),
                CountReviews = d.Reviews.Count,

            });
            return result;
        }

        public async Task<IEnumerable<MedicineDto>> GetTopRatedMedicationsAsync(int count)
        {
            var topMedications = await _context.Medications
                .Include(m => m.Reviews)
                .OrderByDescending(m => m.Reviews.Average(r => (double?)r.Rating) ?? 0)
                .Take(count)
                .ToListAsync();


            var result = new List<MedicineDto>();


            foreach (var med in topMedications)
            {
                var inventoryItem = await _context.InventoryItems
                    .Include(i => i.Pharmacy)
                        .ThenInclude(p => p.Address)
                    .FirstOrDefaultAsync(i => i.MedicationId == med.Id);

                if (inventoryItem != null)
                    result.Add(MappingExtensions.MapInventoryToDto(inventoryItem));
            }
            return result;

        }

    }

}
