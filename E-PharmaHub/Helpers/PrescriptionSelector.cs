using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Helpers
{
        public static class PrescriptionItemSelector
        {
            public static PrescriptionItemDto ToDto(this PrescriptionItem item)
            {
                return new PrescriptionItemDto
                {
                    Id = item.Id,
                    MedicationName = item.MedicationName,
                    MedicationStrength = item.MedicationStrength,
                    Dosage = item.Dosage,
                    Quantity = item.Quantity,
                    Duration = item.Duration,
                    Notes = item.Notes
                };
            }

            public static PrescriptionItem ToEntity(this PrescriptionItemDto dto)
            {
                return new PrescriptionItem
                {
                    MedicationName = dto.MedicationName,
                    MedicationStrength = dto.MedicationStrength,
                    Dosage = dto.Dosage,
                    Quantity = dto.Quantity,
                    Duration = dto.Duration,
                    Notes = dto.Notes
                };
            }

        public static PrescriptionDto ToDto(this Prescription prescription)
        {
            return new PrescriptionDto
            {
                Id = prescription.Id,
                DoctorId = prescription.DoctorId,
                IssuedAt = prescription.IssuedAt,
                Items = prescription.Items?
                    .Select(i => i.ToDto())
                    .ToList() ?? new List<PrescriptionItemDto>()
            };
        }

        public static Prescription ToEntity(
            this CreatePrescriptionDto dto,
            string userId)
        {
            return new Prescription
            {
                UserId = userId,
                DoctorId = dto.DoctorId ?? 0,
                Items = dto.Items?
                    .Select(i => i.ToEntity())
                    .ToList() ?? new List<PrescriptionItem>()
            };
        }
    }
}