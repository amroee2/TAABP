using TAABP.Application.DTOs;
using TAABP.Core;

namespace TAABP.Application.Profile.ReviewMapping
{
    public interface IReviewMapper
    {
        void ReviewDtoToReview(ReviewDto reviewDto, Review review);
        ReviewDto ReviewToReviewDto(Review review);
    }
}
