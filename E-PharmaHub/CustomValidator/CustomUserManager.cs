using E_PharmaHub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace E_PharmaHub.CustomValidator
{
    public class CustomUserManager : UserManager<AppUser>
    {
        public CustomUserManager(
            IUserStore<AppUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<AppUser> passwordHasher,
            IEnumerable<IUserValidator<AppUser>> userValidators,
            IEnumerable<IPasswordValidator<AppUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<AppUser>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators,
                   passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        public override async Task<IdentityResult> CreateAsync(AppUser user, string password)
        {
            user.NormalizedUserName = user.Email.ToUpper();
            return await base.CreateAsync(user, password);
        }
    }

}
