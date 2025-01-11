using FluentValidation;
using TAABP.Application.DTOs;

namespace TAABP.Application.Validators
{
    public class ReviewDtoValidator : AbstractValidator<ReviewDto>
    {
        public ReviewDtoValidator()
        {
            RuleFor(x => x.Rating)
                .NotEmpty()
                .WithMessage("Rating is required")
                .InclusiveBetween(0, 5)
                .WithMessage("Rating must be between 0 and 5");
            RuleFor(x => x.Comment)
                .NotEmpty()
                .WithMessage("Comment is required")
                .MaximumLength(100)
                .WithMessage("Comment must not exceed 100 characters");
        }
    }
}
