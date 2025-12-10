using E_PharmaHub.Models;
using Microsoft.AspNetCore.Identity;

namespace E_PharmaHub.CustomValidator
{
    public class DuplicateUsernameAllowedValidator : UserValidator<AppUser>
    {
        public override async Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            var errors = new List<IdentityError>();

            var baseResult = await base.ValidateAsync(manager, user);

            errors.AddRange(baseResult.Errors
                .Where(e => !e.Code.Contains("InvalidUserName")));

            return errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
        }
    }


}
