using E_PharmaHub.Models;
using E_PharmaHub.UnitOfWorkes;

namespace E_PharmaHub.Services.AddressServ
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddressService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Address>> GetAllAddressesAsync()
        {
            return await _unitOfWork.Addresses.GetAllAsync();
        }

        public async Task<Address> GetAddressByIdAsync(int id)
        {
            return await _unitOfWork.Addresses.GetByIdAsync(id);
        }

        public async Task AddAddressAsync(Address address)
        {
            await _unitOfWork.Addresses.AddAsync(address);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateAddressAsync(int id, Address address)
        {
            var existing = await _unitOfWork.Addresses.GetByIdAsync(id);
            if (existing == null) throw new Exception("Address not found");

            existing.Country = address.Country;
            existing.City = address.City;
            existing.Street = address.Street;
            existing.PostalCode = address.PostalCode;
            existing.Latitude = address.Latitude;
            existing.Longitude = address.Longitude;

            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAddressAsync(int id)
        {
            var address = await _unitOfWork.Addresses.GetByIdAsync(id);
            if (address != null)
            {
                _unitOfWork.Addresses.Delete(address);
                await _unitOfWork.CompleteAsync();
            }
        }
    }

}
