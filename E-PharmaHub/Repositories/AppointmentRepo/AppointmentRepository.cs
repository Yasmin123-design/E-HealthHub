using E_PharmaHub.Dtos;
using E_PharmaHub.Helpers;
using E_PharmaHub.Models;
using E_PharmaHub.Models.Enums;
using E_PharmaHub.Repositories.DoctorRepo;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace E_PharmaHub.Repositories.AppointmentRepo
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly EHealthDbContext _context;
        private readonly IDoctorRepository _doctorRepository;
        public AppointmentRepository(EHealthDbContext context,IDoctorRepository doctorRepository)
        {
            _context = context;
            _doctorRepository = doctorRepository;
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
        public async Task<IEnumerable<AppointmentResponseDto>> GetConfirmedAppointmentsByDoctorIdAsync(string doctorId)
        {
            return await BaseAppointmentIncludes()
                .Where(a => a.DoctorId == doctorId && 
                (a.Status == AppointmentStatus.Confirmed ||
                a.Status == AppointmentStatus.Completed)
                )
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

            var appointments = await _context.Appointments
                .CountAsync(a =>
                a.Status == AppointmentStatus.Confirmed &&
                    a.DoctorId == doctorId &&
                    a.StartAt.Date == today);

            return appointments;
        }
        public async Task<int> GetYesterdayAppointmentsCountAsync(string doctorId)
        {
            var yesterday = DateTime.Today.AddDays(-1);

            return await _context.Appointments
                .CountAsync(a =>
                a.Status == AppointmentStatus.Confirmed &&
                    a.DoctorId == doctorId &&
                    a.StartAt.Date == yesterday);
        }
        public async Task<int> GetTotalConfirmedAppointmentsCountAsync(string doctorId)
        {

            return await _context.Appointments
                .CountAsync(a =>
                a.Status == AppointmentStatus.Confirmed &&
                    a.DoctorId == doctorId );
        }
        public async Task<int> GetTotalCancelledAppointmentsCountAsync(string doctorId)
        {

            return await _context.Appointments
                .CountAsync(a =>
                a.Status == AppointmentStatus.Cancelled &&
                    a.DoctorId == doctorId);
        }
        public async Task<int> GetTotalCompletedAppointmentsCountAsync(string doctorId)
        {

            return await _context.Appointments
                .CountAsync(a =>
                a.Status == AppointmentStatus.Completed &&
                    a.DoctorId == doctorId);
        }
        public async Task<int> GetTotalPenddingAppointmentsCountAsync(string doctorId)
        {

            return await _context.Appointments
                .CountAsync(a =>
                a.Status == AppointmentStatus.Pending &&
                    a.DoctorId == doctorId);
        }
        public async Task<IEnumerable<Appointment>> GetBookedByDoctorAndDateAsync(
      int doctorId,
      DateTime date)
        {
            var doctor = await _doctorRepository.GetByIdAsync(doctorId);

            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            return await _context.Appointments
                .Where(a =>
                    a.DoctorId == doctor.AppUserId &&
                    a.StartAt >= startDate &&
                    a.StartAt < endDate &&
                    (a.Status == AppointmentStatus.Pending ||
                     a.Status == AppointmentStatus.Confirmed))
                .ToListAsync();
        }

        public async Task<bool> IsSlotBookedAsync(
            int doctorId,
            DateTime startAt,
            DateTime endAt)
        {
            var doctor = await _doctorRepository.GetByIdAsync(doctorId);

            return await _context.Appointments.AnyAsync(a =>
                a.DoctorId == doctor.AppUserId &&
                a.StartAt == startAt &&
                a.EndAt == endAt &&
                a.Status == AppointmentStatus.Confirmed);
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
        public async Task<decimal> GetYesterRevenueAsync(string doctorId)
        {
            var yesterday = DateTime.Today.AddDays(-1);

            return await _context.Appointments
                .Where(a =>
                    a.DoctorId == doctorId &&
                    a.IsPaid &&
                    a.Status == AppointmentStatus.Confirmed &&
                    a.StartAt.Date == yesterday)
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
        public async Task<List<DailyAppointmentsDto>> GetWeeklyAppointmentsAsync(string doctorId)
        {
            var endDateUtc = DateTime.UtcNow.Date;     
            var startDateUtc = endDateUtc.AddDays(-6); 

            var data = await _context.Appointments
                .Where(a =>
                    a.DoctorId == doctorId &&
                    a.StartAt >= startDateUtc &&
                    a.StartAt < endDateUtc.AddDays(1) &&
                    a.Status == AppointmentStatus.Confirmed)
                .GroupBy(a => a.StartAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var result = Enumerable
                .Range(0, 7)
                .Select(i =>
                {
                    var date = startDateUtc.AddDays(i);
                    var dayData = data.FirstOrDefault(d => d.Date == date);

                    return new DailyAppointmentsDto
                    {
                        Date = date.ToString("yyyy-MM-dd"),
                        AppointmentsCount = dayData?.Count ?? 0
                    };
                })
                .ToList();

            return result;
        }

        public async Task<List<DailyAppointmentsDto>> GetDailyAppointmentsAsync(
    string doctorId,
    int? year,
    int? month)
        {
            DateTime startDateUtc;
            DateTime endDateUtc;

            if (year.HasValue && month.HasValue)
            {
                startDateUtc = new DateTime(year.Value, month.Value, 1);
                endDateUtc = startDateUtc.AddMonths(1).AddDays(-1);
            }
            else if (year.HasValue)
            {
                startDateUtc = new DateTime(year.Value, 1, 1);
                endDateUtc = new DateTime(year.Value, 12, 31);
            }
            else
            {
                startDateUtc = await _context.Appointments
                    .Where(a => a.DoctorId == doctorId)
                    .MinAsync(a => a.StartAt.Date);

                endDateUtc = DateTime.UtcNow.Date;
            }

            var data = await _context.Appointments
                .Where(a =>
                    a.DoctorId == doctorId &&
                    a.StartAt.Date >= startDateUtc &&
                    a.StartAt.Date <= endDateUtc &&
                    a.Status == AppointmentStatus.Confirmed)
                .GroupBy(a => a.StartAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var totalDays = (endDateUtc - startDateUtc).Days + 1;

            var result = Enumerable
                .Range(0, totalDays)
                .Select(i =>
                {
                    var date = startDateUtc.AddDays(i);
                    var dayData = data.FirstOrDefault(d => d.Date == date);

                    return new DailyAppointmentsDto
                    {
                        Date = date.ToString("yyyy-MM-dd"),
                        AppointmentsCount = dayData?.Count ?? 0
                    };
                })
                .ToList();

            return result;
        }

        public async Task<List<DailyRevenueDto>> GetWeeklyRevenueAsync(string doctorId)
        {
            var endDateUtc = DateTime.UtcNow.Date;
            var startDateUtc = endDateUtc.AddDays(-6);

            var data = await _context.Appointments
                .Where(a =>
                    a.DoctorId == doctorId &&
                    a.StartAt >= startDateUtc &&
                    a.StartAt < endDateUtc.AddDays(1) &&
                    a.IsPaid &&
                    a.Status == AppointmentStatus.Confirmed)
                .GroupBy(a => a.StartAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Total = g.Sum(a => a.Payment.Amount)
                })
                .ToListAsync();

            var result = Enumerable
                .Range(0, 7)
                .Select(i =>
                {
                    var date = startDateUtc.AddDays(i);
                    var dayData = data.FirstOrDefault(d => d.Date == date);

                    return new DailyRevenueDto
                    {
                        Date = date.ToString("yyyy-MM-dd"),
                        TotalRevenue = dayData?.Total ?? 0
                    };
                })
                .ToList();

            return result;
        }

        public async Task<List<DailyRevenueDto>> GetDailyRevenueAsync(
    string doctorId,
    int? year,
    int? month)
        {
            DateTime startDateUtc;
            DateTime endDateUtc;

            if (year.HasValue && month.HasValue)
            {
                startDateUtc = new DateTime(year.Value, month.Value, 1);
                endDateUtc = startDateUtc.AddMonths(1).AddDays(-1);
            }
            else if (year.HasValue)
            {
                startDateUtc = new DateTime(year.Value, 1, 1);
                endDateUtc = new DateTime(year.Value, 12, 31);
            }
            else
            {
                startDateUtc = await _context.Appointments
                    .Where(a => a.DoctorId == doctorId)
                    .MinAsync(a => a.StartAt.Date);

                endDateUtc = DateTime.UtcNow.Date;
            }

            var data = await _context.Appointments
                .Where(a =>
                    a.DoctorId == doctorId &&
                    a.StartAt.Date >= startDateUtc &&
                    a.StartAt.Date <= endDateUtc &&
                    a.IsPaid &&
                    a.Status == AppointmentStatus.Confirmed)
                .GroupBy(a => a.StartAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Total = g.Sum(a => a.Payment.Amount)
                })
                .ToListAsync();

            var totalDays = (endDateUtc - startDateUtc).Days + 1;

            var result = Enumerable
                .Range(0, totalDays)
                .Select(i =>
                {
                    var date = startDateUtc.AddDays(i);
                    var dayData = data.FirstOrDefault(d => d.Date == date);

                    return new DailyRevenueDto
                    {
                        Date = date.ToString("yyyy-MM-dd"),
                        TotalRevenue = dayData?.Total ?? 0
                    };
                })
                .ToList();

            return result;
        }

        public async Task<GenderStatsDto> GetGenderStatsAsync(string doctorId)
        {
            return new GenderStatsDto
            {
                Male = await _context.Appointments.CountAsync(a =>
                    a.DoctorId == doctorId &&
                    a.PatientGender == Gender.Male),

                Female = await _context.Appointments.CountAsync(a =>
                    a.DoctorId == doctorId &&
                    a.PatientGender == Gender.Female)
            };
        }
        public async Task<List<DailyStatusStatsDto>> GetWeeklyStatusStatsAsync(string doctorId)
        {
            var endDateUtc = DateTime.UtcNow.Date;
            var startDateUtc = endDateUtc.AddDays(-6);

            var data = await _context.Appointments
                .Where(a =>
                    a.DoctorId == doctorId &&
                    a.StartAt >= startDateUtc &&
                    a.StartAt < endDateUtc.AddDays(1))
                .GroupBy(a => a.StartAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Confirmed = g.Count(a => a.Status == AppointmentStatus.Confirmed),
                    Cancelled = g.Count(a => a.Status == AppointmentStatus.Cancelled)
                })
                .ToListAsync();

            var result = Enumerable
                .Range(0, 7)
                .Select(i =>
                {
                    var date = startDateUtc.AddDays(i);
                    var dayData = data.FirstOrDefault(d => d.Date == date);

                    return new DailyStatusStatsDto
                    {
                        Date = date.ToString("yyyy-MM-dd"),
                        Confirmed = dayData?.Confirmed ?? 0,
                        Cancelled = dayData?.Cancelled ?? 0
                    };
                })
                .ToList();

            return result;
        }

        public async Task<List<AgeRangeDto>> GetAgeRangesAsync(string doctorId)
        {
            var appointments = await _context.Appointments
                .Where(a => a.DoctorId == doctorId)
                .ToListAsync();

            return new List<AgeRangeDto>
    {
        new() { Range = "0 - 5", Count = appointments.Count(a => a.PatientAge <= 5) },
        new() { Range = "6 - 12", Count = appointments.Count(a => a.PatientAge >= 6 && a.PatientAge <= 12) },
        new() { Range = "13 - 18", Count = appointments.Count(a => a.PatientAge >= 13 && a.PatientAge <= 18) },
        new() { Range = "19 - 25", Count = appointments.Count(a => a.PatientAge >= 19 && a.PatientAge <= 25) },
        new() { Range = "26 - 35", Count = appointments.Count(a => a.PatientAge >= 26 && a.PatientAge <= 35) },
        new() { Range = "36 - 45", Count = appointments.Count(a => a.PatientAge >= 36 && a.PatientAge <= 45) },
        new() { Range = "46 - 60", Count = appointments.Count(a => a.PatientAge >= 46 && a.PatientAge <= 60) },
        new() { Range = "60+", Count = appointments.Count(a => a.PatientAge >= 60) }
    };
    }  
    }
}
