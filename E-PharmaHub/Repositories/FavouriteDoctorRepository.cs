using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace E_PharmaHub.Repositories
{
    public class FavouriteDoctorRepository : IFavouriteDoctorRepository
    {
        private readonly EHealthDbContext _context;

        public FavouriteDoctorRepository(EHealthDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddToFavoritesAsync(string userId, int doctorId)
        {
            var exists = await _context.FavouriteDoctors
                .AnyAsync(f => f.UserId == userId && f.DoctorId == doctorId);

            if (exists)
                return false;

            var favorite = new FavoriteDoctor
            {
                UserId = userId,
                DoctorId = doctorId
            };

            await _context.FavouriteDoctors.AddAsync(favorite);
            return true;
        }

        public async Task<bool> RemoveFromFavoritesAsync(string userId, int doctorId)
        {
            var favorite = await _context.FavouriteDoctors
                .FirstOrDefaultAsync(f => f.UserId == userId && f.DoctorId == doctorId);

            if (favorite == null)
                return false;

            _context.FavouriteDoctors.Remove(favorite);
            return true;
        }

        public async Task<IEnumerable<DoctorReadDto>> GetUserFavoritesAsync(string userId)
        {
            var favorites = await _context.FavouriteDoctors
                .Where(f => f.UserId == userId)
                .Include(f => f.Doctor)
                    .ThenInclude(d => d.AppUser)
                .Include(f => f.Doctor)
                    .ThenInclude(d => d.Clinic)
                        .ThenInclude(c => c.Address)
                .Include(f => f.Doctor)
                    .ThenInclude(d => d.Reviews)
                .ToListAsync();

            var result = favorites.Select(f =>
            {
                var doctor = f.Doctor;
                return new DoctorReadDto
                {
                    Id = doctor.Id,
                    Email = doctor.AppUser.Email,
                    Specialty = doctor.Specialty,
                    IsApproved = doctor.IsApproved,
                    Gender = doctor.Gender,
                    AverageRating = doctor.Reviews.Any() ? doctor.Reviews.Average(r => r.Rating) : 0,
                    Username = doctor.AppUser.UserName,
                    ConsultationPrice = doctor.ConsultationPrice,
                    ConsultationType = doctor.ConsultationType,
                    ClinicName = doctor.Clinic.Name,
                    ClinicPhone = doctor.Clinic.Phone,
                    ClinicImagePath = doctor.Clinic.ImagePath,
                    DoctorImage = doctor.AppUser.ProfileImage,
                    City = doctor.Clinic.Address.City,
                    PostalCode = doctor.Clinic.Address.PostalCode,
                    Country = doctor.Clinic.Address.Country,
                    Street = doctor.Clinic.Address.Street,
                    Latitude = doctor.Clinic.Address.Latitude,
                    Longitude = doctor.Clinic.Address.Longitude,
                    CountPatient = _context.Appointments.Count(a => a.DoctorId == doctor.AppUserId),          
                    CountReviews = doctor.Reviews.Count,
                    CountFavourite = _context.FavouriteDoctors.Count(fd => fd.DoctorId == doctor.Id)
                };
            });

            return result;
        }


        public async Task<int> CountByDoctorIdAsync(int doctorId)
        {
            return await _context.FavouriteDoctors
                .CountAsync(f => f.DoctorId == doctorId);
        }
    }
}

