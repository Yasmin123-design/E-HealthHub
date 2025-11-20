using E_PharmaHub.Dtos;
using E_PharmaHub.Helpers;
using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace E_PharmaHub.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly EHealthDbContext _context;

        public AppointmentRepository(EHealthDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Appointment entity)
        {
            await _context.Appointments.AddAsync(entity);
        }


        public void Delete(Appointment entity)
        {
            _context.Appointments.Remove(entity);
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _context.Appointments
                .Include(a => a.UserId)
                .Include(a => a.Clinic)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<Appointment, bool>> predicate)
        {
            return await _context.Appointments.AnyAsync(predicate);
        }


        public async Task<Appointment> GetByIdAsync(int id)
        {
            return await _context.Appointments
                .Include(u => u.User)
                .Include(a => a.Doctor)
                .Include(a => a.Clinic)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task Update(Appointment entity)
        {
             _context.Appointments.Update(entity);
        }

        private IQueryable<Appointment> BaseAppointmentIncludes()
        {
            return _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Doctor)
                .Include(a => a.Clinic)
                .AsNoTracking();
        }

        private Expression<Func<Appointment, AppointmentResponseDto>> Selector =>
            AppointmentSelectors.GetAppointmentSelector();

        public async Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsByDoctorIdAsync(string doctorId)
        {
            return await BaseAppointmentIncludes()
                .Where(a => a.DoctorId == doctorId)
                .Select(Selector)
                .ToListAsync();
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsByUserIdAsync(string userId)
        {
            return await BaseAppointmentIncludes()
                .Where(a => a.UserId == userId)
                .Select(Selector)
                .ToListAsync();
        }

        public async Task<AppointmentResponseDto?> GetAppointmentResponseByIdAsync(int id)
        {
            return await BaseAppointmentIncludes()
                .Where(a => a.Id == id)
                .Select(Selector)
                .FirstOrDefaultAsync();
        }
        public async Task<AppointmentResponseDto> AddAppointmentAndReturnResponseAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();

            return await BaseAppointmentIncludes()
                .Where(a => a.Id == appointment.Id)
                .Select(Selector)
                .FirstAsync();
        }


    }
}
