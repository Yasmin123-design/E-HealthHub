using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;

namespace E_PharmaHub.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly EHealthDbContext _context;

        public UserRepository(EHealthDbContext context)
        {
            _context = context;
        }

        public async Task<AppUser?> GetByIdAsync(string userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public void Update(AppUser user)
        {
            _context.Users.Update(user);
        }

        public void Delete(AppUser user)
        {
            _context.Users.Remove(user);
        }
    }
}

