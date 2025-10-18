using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;

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


        public async Task<Appointment> BookAppointmentAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
            return appointment;

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

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDoctorIdAsync(string doctorId)
        {
            return await _context
                .Appointments
                .Include(a => a.User)
                .Include(a => a.Clinic)
                .Where(a => a.DoctorId == doctorId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByUserIdAsync(string userId)
        {
            return await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Doctor)
                .Include(a => a.Clinic)
                .Where(a => a.UserId == userId)
                .ToListAsync();
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
    }
}
