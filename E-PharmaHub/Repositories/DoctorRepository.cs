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

        public async Task<IEnumerable<DoctorProfile>> GetAllAsync()
        {
            return await _context.DoctorProfiles
                .Include(d => d.Clinic)
                .Include(d => d.AppUser)
                .ToListAsync();
        }

        public async Task<DoctorReadDto?> GetDoctorByUserIdAsync(string userId)
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
                    City = d.Clinic.Address.City
                })
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
                    City = d.Clinic.Address.City
                })
                .FirstOrDefaultAsync();
        }


        public async Task AddAsync(DoctorProfile entity)
        {
            await _context.DoctorProfiles.AddAsync(entity);
        }

        public void Update(DoctorProfile entity)
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
                    City = d.Clinic.Address.City
                })
                .ToListAsync();
        }

        public async Task<bool> ApproveDoctorAsync(int id)
        {
            var doctor = await _context.DoctorProfiles.FindAsync(id);
            if (doctor == null || doctor.IsApproved)
                return false;

            doctor.IsApproved = true;
            return true;
        }

        public async Task<bool> RejectDoctorAsync(int id)
        {
            var doctor = await _context.DoctorProfiles.FindAsync(id);
            if (doctor == null || !doctor.IsApproved)
                return false;

            doctor.IsApproved = false;
            return true;
        }
    }
}
