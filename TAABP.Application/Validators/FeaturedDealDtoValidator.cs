using FluentValidation;
using TAABP.Application.DTOs;

namespace TAABP.Application.Validators
{
    public class FeaturedDealDtoValidator : AbstractValidator<FeatueredDealDto>
    {
        public FeaturedDealDtoValidator()
        {
            RuleFor(x => x.RoomId)
                .NotEmpty().WithMessage("{PropertyName} is required")
                .GreaterThan(0).WithMessage("{PropertyName} must be greater than 0");
            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("{PropertyName} is required")
                .GreaterThan(DateTime.Now).WithMessage("{PropertyName} must be greater than today");
            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("{PropertyName} is required")
                .GreaterThan(x => x.StartDate).WithMessage("{PropertyName} must be greater than Start Date");
            RuleFor(x => x.Discount)
                .NotEmpty().WithMessage("{PropertyName} is required")
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} must be greater than 0");
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("{PropertyName} is required")
                .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("{PropertyName} is required")
                .MaximumLength(500).WithMessage("{PropertyName} must not exceed 500 characters");
        }
    }
}
