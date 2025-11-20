using E_PharmaHub.Dtos;
using E_PharmaHub.Helpers;
using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace E_PharmaHub.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly EHealthDbContext _context;

        public DoctorRepository(EHealthDbContext context)
        {
            _context = context;
        }

        private IQueryable<DoctorProfile> BaseDoctorIncludes()
        {
            return _context.DoctorProfiles
                .Include(d => d.AppUser)
                .Include(d => d.Clinic)
                    .ThenInclude(c => c.Address)
                .Include(d => d.Reviews)
                .AsNoTracking();
        }

        private Expression<Func<DoctorProfile, DoctorReadDto>> Selector =>
            DoctorSelectors.GetDoctorSelector(
                _context.Appointments,
                _context.FavouriteDoctors
            );

        public async Task AddAsync(DoctorProfile entity)
        {
            await _context.DoctorProfiles.AddAsync(entity);
        }

        public async Task Update(DoctorProfile entity)
        {
            _context.DoctorProfiles.Update(entity);
        }

        public void Delete(DoctorProfile entity)
        {
            _context.DoctorProfiles.Remove(entity);
        }

        public async Task<IEnumerable<DoctorProfile>> GetAllAsync()
        {
            return await BaseDoctorIncludes().ToListAsync();
        }

        public async Task<DoctorProfile?> GetDoctorProfileByIdAsync(int id)
        {
            return await BaseDoctorIncludes()
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<DoctorProfile?> GetDoctorByUserIdAsync(string userId)
        {
            return await BaseDoctorIncludes()
                .FirstOrDefaultAsync(d => d.AppUserId == userId);
        }

        public async Task<IEnumerable<DoctorReadDto>> GetAllDoctorsShowToAdminAsync()
        {
            return await BaseDoctorIncludes()
                .Select(Selector)
                .ToListAsync();
        }

        public async Task<IEnumerable<DoctorReadDto>> GetAllDoctorsAcceptedByAdminAsync()
        {
            return await BaseDoctorIncludes()
                .Where(d => d.IsApproved)
                .Select(Selector)
                .ToListAsync();
        }

        public async Task<DoctorReadDto?> GetDoctorByUserIdReadDtoAsync(string userId)
        {
            return await BaseDoctorIncludes()
                .Where(d => d.AppUserId == userId)
                .Select(Selector)
                .FirstOrDefaultAsync();
        }

        public async Task<DoctorReadDto?> GetByIdDetailsAsync(int id)
        {
            return await BaseDoctorIncludes()
                .Where(d => d.Id == id)
                .Select(Selector)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<DoctorReadDto>> GetFilteredDoctorsAsync(
                             string? name, Gender? gender, string? sortOrder, ConsultationType? consultationType)
        {
            var query = BaseDoctorIncludes();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(d => d.AppUser.UserName.Contains(name));

            if (gender.HasValue)
                query = query.Where(d => d.Gender == gender);

            if (consultationType.HasValue)
                query = query.Where(d => d.ConsultationType == consultationType);

            query = sortOrder switch
            {
                "PriceLowToHigh" => query.OrderBy(d => d.ConsultationPrice),
                "PriceHighToLow" => query.OrderByDescending(d => d.ConsultationPrice),
                _ => query
            };

            return await query.Select(Selector).ToListAsync();
        }

        public async Task<IEnumerable<DoctorReadDto>> GetDoctorsBySpecialtyAsync(string specialty)
        {
            return await BaseDoctorIncludes()
                .Where(d => d.Specialty == specialty)
                .Select(Selector)
                .ToListAsync();
        }

        public async Task<bool> ApproveDoctorAsync(int id)
        {
            var doctor = await _context.DoctorProfiles.FindAsync(id);
            if (doctor == null || doctor.IsApproved)
                return false;

            doctor.IsApproved = true;
            doctor.IsRejected = false;
            return true;
        }

        public async Task<bool> RejectDoctorAsync(int id)
        {
            var doctor = await _context.DoctorProfiles.FindAsync(id);
            if (doctor == null || doctor.IsRejected)
                return false;

            doctor.IsRejected = true;
            doctor.IsApproved = false;
            return true;
        }

        public async Task MarkAsPaid(string userId)
        {
            var doctor = await _context.DoctorProfiles
                .FirstOrDefaultAsync(d => d.AppUserId == userId);

            if (doctor == null)
                throw new Exception("Doctor profile not found.");

            doctor.HasPaid = true;
        }

        public async Task<DoctorProfile> GetByIdAsync(int id)
        {
            return await BaseDoctorIncludes()
                           .FirstOrDefaultAsync(d => d.Id == id);
        }
        public async Task<IEnumerable<DoctorReadDto>> GetTopRatedDoctorsAsync(int count)
        {
            var doctors = await BaseDoctorIncludes()
                .OrderByDescending(d =>
                    d.Reviews.Any()
                        ? d.Reviews.Average(r => (double?)r.Rating)
                        : 0
                )
                .Take(count)
                .Select(Selector)
                .ToListAsync();

            return doctors;
        }

    }
}
