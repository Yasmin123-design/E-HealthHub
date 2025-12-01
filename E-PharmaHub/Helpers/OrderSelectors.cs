using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using System.Linq.Expressions;

namespace E_PharmaHub.Helpers
{
    public static class OrderSelectors
    {
        public static Expression<Func<Order, OrderResponseDto>> GetOrderSelector()
        {
            return o => new OrderResponseDto
            {
                Id = o.Id,
                UserId = o.UserId,
                UserEmail = o.User.Email,
                UserImage = o.User.ProfileImage,
                UserName = o.User.UserName,
                PharmacyId = o.PharmacyId,
                PharmacyName = o.Pharmacy.Name,
                PharmacyImage = o.Pharmacy.ImagePath,
                PhoneNumber = o.PhoneNumber,
                City = o.City,
                Country = o.Country,
                Street = o.Street,
                TotalPrice = o.TotalPrice,
                Status = o.Status.ToString(),
                PaymentStatus = o.PaymentStatus.ToString(),
                CreatedAt = o.CreatedAt,

                Items = o.Items.Select(i => new OrderResponseItemDto
                {
                    MedicationId = i.MedicationId,
                    MedicationName = i.Medication.BrandName,
                    MedicicationImage = i.Medication.ImagePath,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };
        }
    }
}
