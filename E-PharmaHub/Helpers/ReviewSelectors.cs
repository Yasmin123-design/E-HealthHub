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
                UserName = r.User.UserName,
                UserId = r.User.Id,
                Image = r.User.ProfileImage,
                Rating = r.Rating,
                Comment = r.Comment,
                UserEmail = r.User.Email
            };
    }
}
