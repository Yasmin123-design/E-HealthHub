using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories.UserRepo
{
    public interface IUserRepository
    {
        Task<AppUser?> GetByIdAsync(string userId);
        void Update(AppUser user);
        void Delete(AppUser user);
    }
}
