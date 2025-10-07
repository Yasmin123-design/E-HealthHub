using E_PharmaHub.Models;
using System.Linq.Expressions;

namespace E_PharmaHub.Repositories
{
    public interface IAddressRepository : IGenericRepository<Address>
    {
        Task<Address?> FindAsync(Expression<Func<Address, bool>> predicate);

    }
}
