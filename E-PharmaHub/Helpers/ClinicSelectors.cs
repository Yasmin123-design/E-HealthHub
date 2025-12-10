using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using System.Linq.Expressions;

namespace E_PharmaHub.Helpers
{
    public static class ClinicSelectors
    {
        public static Expression<Func<Clinic, ClinicDto>> ClinicDtoSelector =>
        c => new ClinicDto
        {
            Id = c.Id,
            Name = c.Name,
            Phone = c.Phone,
            ImagePath = c.ImagePath,
            AddressId = c.AddressId??0,
            City = c.Address.City,
            Street = c.Address.Street,
            Country = c.Address.Country,
            PostalCode = c.Address.PostalCode,
            Latitude = c.Address.Latitude,
            Longitude = c.Address.Longitude
        };

        public static ClinicDto MapToDto(Clinic clinic)
        {
            if (clinic == null) return null;
            return new ClinicDto
            {
                Id = clinic.Id,
                Name = clinic.Name,
                Phone = clinic.Phone,
                ImagePath = clinic.ImagePath,
                AddressId = clinic.AddressId??0,
                City = clinic.Address?.City,
                Street = clinic.Address?.Street,
                Country = clinic.Address?.Country,
                PostalCode = clinic.Address?.PostalCode,
                Latitude = clinic.Address?.Latitude,
                Longitude = clinic.Address?.Longitude
            };
        }
    }
}
