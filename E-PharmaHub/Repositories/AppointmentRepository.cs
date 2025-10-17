using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly EHealthDbContext _context;

        public AppointmentRepository(EHealthDbContext context)
        {
            _context = context;
        }
        public async Task<Appointment> BookAppointmentAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
            return appointment;

        }
    }
}
