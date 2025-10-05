using E_PharmaHub.Models;

namespace E_PharmaHub.Services
{
    public interface IAddressService
    {
        Task<IEnumerable<Address>> GetAllAddressesAsync();
        Task<Address> GetAddressByIdAsync(int id);
        Task AddAddressAsync(Address address);
        Task UpdateAddressAsync(int id ,Address address);
        Task DeleteAddressAsync(int id);
    }

}
