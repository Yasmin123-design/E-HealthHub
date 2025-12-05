using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using System.Linq.Expressions;

namespace E_PharmaHub.Helpers
{
    public class PharmacySelectors
    {
        public static Expression<Func<Pharmacy, PharmacySimpleDto>> PharmacySimpleDtoSelector =>
           p => new PharmacySimpleDto
           {
               Id = p.Id,
               Name = p.Name,
               Phone = p.Phone,
               City = p.Address.City,
               ImagePath = p.ImagePath,
               DeliveryFee = p.DeliveryFee,
               Country = p.Address.Country,
               Latitude = p.Address.Latitude,
               Longitude = p.Address.Longitude,
               PostalCode = p.Address.PostalCode,
               Street = p.Address.Street,
               AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0
           };
    }
}

