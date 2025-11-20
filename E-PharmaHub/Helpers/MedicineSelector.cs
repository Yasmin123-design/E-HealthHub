using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Helpers
{
    public static class MedicineSelector
    {
        public static MedicineDto MapInventoryToDto(InventoryItem item)
        {
            if (item == null) return null;

            return new MedicineDto
            {
                Id = item.Medication.Id,
                BrandName = item.Medication.BrandName,
                GenericName = item.Medication.GenericName,
                ATCCode = item.Medication.ATCCode,
                Strength = item.Medication.Strength,
                DosageForm = item.Medication.DosageForm,
                ImagePath = item.Medication.ImagePath,
                Price = item.Price,
                Description = item.Medication.Description,
                Warning = item.Medication.Warning,
                Composition = item.Medication.Composition,
                DirectionsForUse = item.Medication.DirectionsForUse,
                SuitableFor = item.Medication.SuitableFor,
                NotSuitableFor = item.Medication.NotSuitableFor,
                Quantity = item.Quantity,
                AverageRating = item.Medication.Reviews.Any() ? item.Medication.Reviews.Average(r => r.Rating) : 0,
                Pharmacy = new PharmacySimpleDto
                {
                    Id = item.Pharmacy.Id,
                    Name = item.Pharmacy.Name,
                    City = item.Pharmacy.Address?.City,
                    ImagePath = item.Pharmacy.ImagePath,
                    Phone = item.Pharmacy.Phone,
                    PostalCode = item.Pharmacy.Address?.PostalCode,
                    Street = item.Pharmacy.Address?.Street,
                    Country = item.Pharmacy.Address?.Country,
                    Latitude = item.Pharmacy.Address?.Latitude,
                    Longitude = item.Pharmacy.Address?.Longitude
                }
            };
        }
    }
}
