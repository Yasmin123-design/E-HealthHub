using E_PharmaHub.Dtos;
using E_PharmaHub.Helpers;
using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace E_PharmaHub.Repositories.AppointmentRepo
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
            return await BaseAppointmentIncludes().ToListAsync();
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
                .Include(a => a.Clinic)
                .Include(a => a.Doctor)
                .ThenInclude(a => a.DoctorProfile)
                .AsNoTracking();
        }
        public async Task<List<Appointment>> GetPatientsOfDoctorAsync(string doctorId)
        {
            return await BaseAppointmentIncludes()
                .Where(a => a.DoctorId == doctorId)
                .ToListAsync();
        }
        public async Task<Appointment> GetAppointmentByPaymentIdAsync(int paymentId)
        {
            return await BaseAppointmentIncludes()
                .Where(a => a.PaymentId == paymentId)
                .FirstOrDefaultAsync();
        }
        private Expression<Func<Appointment, AppointmentResponseDto>> Selector =>
            AppointmentSelectors.GetAppointmentSelector();

        public async Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsByDoctorIdAsync(string doctorId)
        {
            return await BaseAppointmentIncludes()
                .Where(a => a.DoctorId == doctorId && a.PaymentId != null && a.Payment.PaymentIntentId != null)
                .Select(Selector)
                .ToListAsync();
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsByUserIdAsync(string userId)
        {
            return await BaseAppointmentIncludes()
                .Where(a => a.UserId == userId && a.PaymentId != null)
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

        public async Task<int> GetTodayAppointmentsCountAsync(string doctorId)
        {
            var today = DateTime.Today;

            return await _context.Appointments
                .CountAsync(a =>
                a.Status == AppointmentStatus.Confirmed &&
                    a.DoctorId == doctorId &&
                    a.StartAt.Date == today);
        }
        public async Task<int> GetTotalAppointmentsCountAsync(string doctorId)
        {

            return await _context.Appointments
                .CountAsync(a =>
                a.Status == AppointmentStatus.Confirmed &&
                    a.DoctorId == doctorId );
        }
        public async Task<IEnumerable<AppointmentResponseDto>> GetByStatusAsync(
    AppointmentStatus status)
        {
            return await BaseAppointmentIncludes()
                .Where(a => a.Status == status)
                .Select(Selector)
                .ToListAsync();
        }

        public async Task<int> GetTotalPatientsCountAsync(string doctorId)
        {
            return await _context.Appointments
                .Where(a => a.DoctorId == doctorId && a.Status == AppointmentStatus.Confirmed)
                .Select(a => a.UserId)
                .Distinct()
                .CountAsync();
        }

        public async Task<decimal> GetTodayRevenueAsync(string doctorId)
        {
            var today = DateTime.Today;

            return await _context.Appointments
                .Where(a =>
                    a.DoctorId == doctorId &&
                    a.IsPaid &&
                    a.Status == AppointmentStatus.Confirmed &&
                    a.StartAt.Date == today)
                .SumAsync(a => a.Payment!.Amount);
        }

        public async Task<decimal> GetTotalRevenueAsync(string doctorId)
        {
            return await _context.Appointments
                .Where(a =>
                    a.DoctorId == doctorId && 
                    a.Status == AppointmentStatus.Confirmed &&
                    a.IsPaid)
                .SumAsync(a => a.Payment!.Amount);
        }

    }
}
