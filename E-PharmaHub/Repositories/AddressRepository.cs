using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace E_PharmaHub.Repositories
{
    public class AddressRepository : IGenericRepository<Address>
    {
        private readonly EHealthDbContext _context;

        public AddressRepository(EHealthDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Address>> GetAllAsync()
        {
            return await _context.Addresses.ToListAsync();
        }

        public async Task<Address> GetByIdAsync(int id)
        {
            return await _context.Addresses.FindAsync(id);
        }

        public async Task AddAsync(Address entity)
        {
            await _context.Addresses.AddAsync(entity);
        }

        public void Update(Address entity)
        {
            _context.Addresses.Update(entity);
        }

        public void Delete(Address entity)
        {
            _context.Addresses.Remove(entity);
        }
    }

}
