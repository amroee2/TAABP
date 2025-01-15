using Riok.Mapperly.Abstractions;
using TAABP.Application.DTOs;
using TAABP.Core;

namespace TAABP.Application.Profile.ReviewMapping
{
    [Mapper]
    public partial class ReviewMapper : IReviewMapper
    {
        public partial void ReviewDtoToReview(ReviewDto reviewDto, Review review);
        public partial ReviewDto ReviewToReviewDto(Review review);
    }
}
