using E_PharmaHub.Dtos;
using E_PharmaHub.Models;
using System.Linq.Expressions;

namespace E_PharmaHub.Helpers
{
    public static class ReviewSelectors
    {
        public static Expression<Func<Review, ReviewDto>> ReviewDtoSelector =>
            r => new ReviewDto
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = r.Comment,
                User = new UserProfileDto
                {
                    Id = r.User.Id,
                    Address = r.User.Address,
                    ProfileImage = r.User.ProfileImage,
                    Email = r.User.Email,
                    PhoneNumber = r.User.PhoneNumber,
                    UserName = r.User.UserName
                }
            };
    }
}
