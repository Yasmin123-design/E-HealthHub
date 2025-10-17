using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace E_PharmaHub.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly EHealthDbContext _context;

        public DoctorRepository(EHealthDbContext context)
        {
            _context = context;
        }

        public async Task MarkAsPaid(string userId)
        {
            var doctor = await _context.DoctorProfiles
                .FirstOrDefaultAsync(d => d.AppUserId == userId);

            if (doctor == null)
                throw new Exception("Doctor profile not found.");

            doctor.HasPaid = true;
        }
        public async Task<IEnumerable<DoctorReadDto>> GetAllDoctorsShowToAdminAsync()
        {
            return await _context.DoctorProfiles
                .Include(d => d.Clinic)
                .Include(d => d.AppUser)
                                .Select(d => new DoctorReadDto
                                {
                                    Id = d.Id,
                                    Email = d.AppUser.Email,
                                    Specialty = d.Specialty,
                                    IsApproved = d.IsApproved,
                                    ClinicName = d.Clinic.Name,
                                    ClinicPhone = d.Clinic.Phone,
                                    ClinicImagePath = d.Clinic.ImagePath,
                                    City = d.Clinic.Address.City,
                                    DoctorImage = d.Image,
                                    Gender = d.Gender,
                                    ConsultationType = d.ConsultationType,
                                    ConsultationPrice = d.ConsultationPrice
                                })
                .ToListAsync();
        }

        public async Task<IEnumerable<DoctorReadDto>> GetAllDoctorsAcceptedByAdminAsync()
        {
            return await _context.DoctorProfiles
                .Where(d => d.IsApproved) 
                .Include(d => d.Clinic)
                .Include(d => d.AppUser)
                .Select(d => new DoctorReadDto
                {
                    Id = d.Id,
                    Email = d.AppUser.Email,
                    Specialty = d.Specialty,
                    IsApproved = d.IsApproved,
                    ClinicName = d.Clinic.Name,
                    ClinicPhone = d.Clinic.Phone,
                    ClinicImagePath = d.Clinic.ImagePath,
                    City = d.Clinic.Address.City,
                    DoctorImage = d.Image,
                    Gender = d.Gender,
                    ConsultationType = d.ConsultationType,
                    ConsultationPrice = d.ConsultationPrice
                })
                .ToListAsync();
        }

        public async Task<DoctorReadDto?> GetDoctorByUserIdReadDtoAsync(string userId)
        {
            return await _context.DoctorProfiles
                .Include(d => d.AppUser)
                .Include(d => d.Clinic)
                .ThenInclude(c => c.Address)
                .Where(d => d.AppUserId == userId)
                .Select(d => new DoctorReadDto
                {
                    Id = d.Id,
                    Email = d.AppUser.Email,
                    Specialty = d.Specialty,
                    IsApproved = d.IsApproved,
                    ClinicName = d.Clinic.Name,
                    ClinicPhone = d.Clinic.Phone,
                    ClinicImagePath = d.Clinic.ImagePath,
                    City = d.Clinic.Address.City,
                    DoctorImage=d.Image,
                    Gender= d.Gender,
                    ConsultationType=d.ConsultationType,
                    ConsultationPrice=d.ConsultationPrice
                })
                .FirstOrDefaultAsync();
        }
        public async Task<DoctorProfile?> GetDoctorByUserIdAsync(string userId)
        {
            return await _context.DoctorProfiles
                .Include(d => d.AppUser)
                .Include(d => d.Clinic)
                .ThenInclude(c => c.Address)
                .Where(d => d.AppUserId == userId)
                .FirstOrDefaultAsync();
        }
        public async Task<DoctorProfile> GetByIdAsync(int id) 
        { 
            return await _context.DoctorProfiles
                .Include(d => d.AppUser)
                .Include(d => d.Clinic)
                .ThenInclude(d => d.Address)
                .FirstOrDefaultAsync(d => d.Id == id);
        }
        public async Task<DoctorReadDto?> GetByIdDetailsAsync(int id)
        {
            return await _context.DoctorProfiles
                .Include(d => d.AppUser)
                .Include(d => d.Clinic)
                .ThenInclude(c => c.Address)
                .Where(d => d.Id == id)
                .Select(d => new DoctorReadDto
                {
                    Id = d.Id,
                    Email = d.AppUser.Email,
                    Specialty = d.Specialty,
                    IsApproved = d.IsApproved,
                    ClinicName = d.Clinic.Name,
                    ClinicPhone = d.Clinic.Phone,
                    ClinicImagePath = d.Clinic.ImagePath,
                    City = d.Clinic.Address.City,
                    DoctorImage=d.Image,
                    Gender = d.Gender,
                    ConsultationType = d.ConsultationType,
                    ConsultationPrice = d.ConsultationPrice
                })
                .FirstOrDefaultAsync();
        }

        public async Task<DoctorProfile?> GetDoctorProfileByIdAsync(int id)
        {
            return await _context.DoctorProfiles
                .Include(d => d.AppUser)
                .Include(d => d.Clinic)
                .ThenInclude(c => c.Address)
                .Where(d => d.Id == id)
                .FirstOrDefaultAsync();
        }

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

        public async Task<IEnumerable<DoctorReadDto>> GetDoctorsBySpecialtyAsync(string specialty)
        {
            return await _context.DoctorProfiles
                .Include(d => d.AppUser)
                .Include(d => d.Clinic)
                .ThenInclude(c => c.Address)
                .Where(d => d.Specialty == specialty)
                .Select(d => new DoctorReadDto
                {
                    Id = d.Id,
                    Email = d.AppUser.Email,
                    Specialty = d.Specialty,
                    IsApproved = d.IsApproved,
                    ClinicName = d.Clinic.Name,
                    ClinicPhone = d.Clinic.Phone,
                    ClinicImagePath = d.Clinic.ImagePath,
                    City = d.Clinic.Address.City,
                    DoctorImage=d.Image,
                    Gender = d.Gender,
                    ConsultationType = d.ConsultationType,
                    ConsultationPrice = d.ConsultationPrice
                })
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

        public async Task<DoctorProfile> GetDoctorByIdAsync(int id)
        {
            return  _context.DoctorProfiles
                          .Include(d => d.Clinic)
                          .Include(d => d.AppUser)
                          .FirstOrDefault(x => x.Id == id);
        }

        public async Task<IEnumerable<DoctorReadDto>> GetFilteredDoctorsAsync(
                             string? name, Gender? gender, string? sortOrder, ConsultationType? consultationType)
        {
            var query = _context.DoctorProfiles
                .Include(d => d.AppUser)
                .AsQueryable();

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

            return await query
                .Select(d => new DoctorReadDto
                {
                    Id = d.Id,
                    Email = d.AppUser.Email,
                    Specialty = d.Specialty,
                    IsApproved = d.IsApproved,
                    ClinicName = d.Clinic.Name,
                    ClinicPhone = d.Clinic.Phone,
                    ClinicImagePath = d.Clinic.ImagePath,
                    City = d.Clinic.Address.City,
                    DoctorImage = d.Image,
                    Gender = d.Gender,
                    ConsultationType = d.ConsultationType,
                    ConsultationPrice = d.ConsultationPrice
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<DoctorProfile>> GetAllAsync()
        {
            return await _context.DoctorProfiles
    .Include(d => d.Clinic)
    .Include(d => d.AppUser)
    .ToListAsync();
        }
    }
}
