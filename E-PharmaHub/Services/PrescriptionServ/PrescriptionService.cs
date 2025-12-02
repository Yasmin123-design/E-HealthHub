using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using E_PharmaHub.Repositories;
using E_PharmaHub.UnitOfWorkes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace E_PharmaHub.Services.PrescriptionServ
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PrescriptionService(
            IUnitOfWork unitOfWork
            )
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<(bool success, string message, Prescription? prescription)> CreatePrescriptionAsync(CreatePrescriptionDto dto)
        {
            var user = await _unitOfWork.Useres.GetByIdAsync(dto.UserId);
            if (user == null)
                return (false, "User not found", null);

            if (!dto.DoctorId.HasValue || dto.DoctorId <= 0)
                return (false, "DoctorId is required", null);

            var doctor = await _unitOfWork.Doctors.GetByIdAsync(dto.DoctorId.Value);
            if (doctor == null)
                return (false, "Doctor not found", null);

            var hasCompletedAppointment = await _unitOfWork.Appointments.ExistsAsync(a =>
                a.UserId == dto.UserId &&
                a.DoctorId == doctor.AppUserId &&
                a.Status == AppointmentStatus.Completed);

            if (!hasCompletedAppointment)
                return (false, "No completed appointment found between doctor and patient", null);

            var prescription = new Prescription
            {
                UserId = dto.UserId,
                DoctorId = dto.DoctorId.Value,
                Notes = dto.Notes?.Trim(),
                IssuedAt = DateTime.UtcNow,
                Items = dto.Items?.Select(i =>
                {
                    if (i.MedicationId.HasValue)
                    {
                        var medication = _unitOfWork.Medicines.GetByIdAsync(i.MedicationId.Value).Result;
                        if (medication == null)
                            throw new Exception($"Medication ID {i.MedicationId} not found");
                    }
                    else if (string.IsNullOrWhiteSpace(i.MedicationName))
                    {
                        throw new Exception("Either MedicationId or MedicationName must be provided");
                    }

                    return new PrescriptionItem
                    {
                        MedicationId = i.MedicationId,
                        MedicationName = i.MedicationName?.Trim(),
                        Dosage = i.Dosage.Trim(),
                        Quantity = i.Quantity
                    };
                }).ToList() ?? new List<PrescriptionItem>()
            };

            await _unitOfWork.Prescriptions.AddAsync(prescription);
            await _unitOfWork.CompleteAsync();

            return (true, "Prescription created successfully", prescription);
        }


        public async Task<IEnumerable<PrescriptionDetailsDto>> GetUserPrescriptionsAsync(string userId)
        {
            var prescriptions = await _unitOfWork.Prescriptions.GetByUserIdAsync(userId);

            return prescriptions
               .Select(p => new PrescriptionDetailsDto
               {
                   Id = p.Id,
                   UserName = p.User.UserName,
                   DoctorName = p.Doctor.AppUser.UserName,
                   DoctorSpecialty = p.Doctor.Specialty,
                   Notes = p.Notes,
                   IssuedAt = p.IssuedAt,
                   Items = p.Items.Select(i => new PrescriptionItemViewDto
                   {
                       MedicationName = i.MedicationId == null ? i.MedicationName : i.Medication.GenericName,
                       Dosage = i.Dosage,
                       Quantity = i.Quantity
                   }).ToList()
               });

        }

        public async Task<IEnumerable<PrescriptionDetailsDto>> GetDoctorPrescriptionsAsync(int doctorId)
        {
            var prescriptions = await _unitOfWork.Prescriptions.GetByDoctorIdAsync(doctorId);

            return prescriptions
                .Select(p => new PrescriptionDetailsDto
                {
                    Id = p.Id,
                    UserName = p.User.UserName,
                    DoctorName = p.Doctor.AppUser.UserName,
                    DoctorSpecialty = p.Doctor.Specialty,
                    Notes = p.Notes,
                    IssuedAt = p.IssuedAt,
                    Items = p.Items.Select(i => new PrescriptionItemViewDto
                    {
                        MedicationName = i.MedicationId == null ? i.MedicationName : i.Medication.GenericName,
                        Dosage = i.Dosage,
                        Quantity = i.Quantity
                    }).ToList()
                });

        }

        public async Task<PrescriptionDetailsDto?> GetByIdAsync(int id)
        {
            var p = await _unitOfWork.Prescriptions.GetByIdAsync(id);
            if (p == null)
                return null;

            return new PrescriptionDetailsDto
            {
                Id = p.Id,
                UserName = p.User.UserName,
                DoctorName = p.Doctor.AppUser.UserName,
                DoctorSpecialty = p.Doctor.Specialty,
                Notes = p.Notes,
                IssuedAt = p.IssuedAt,
                Items = p.Items.Select(i => new PrescriptionItemViewDto
                {
                    MedicationName = i.MedicationId == null ? i.MedicationName : i.Medication.GenericName,
                    Dosage = i.Dosage,
                    Quantity = i.Quantity
                }).ToList()
            };
        }

        public async Task<bool> DeletePrescriptionAsync(int id)
        {
            var prescription = await _unitOfWork.Prescriptions.GetByIdAsync(id);
            if (prescription == null)
                return false;

            await _unitOfWork.Prescriptions.DeleteAsync(prescription);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
