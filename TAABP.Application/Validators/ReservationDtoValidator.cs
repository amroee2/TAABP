using FluentValidation;
using TAABP.Application.DTOs;

namespace TAABP.Application.Validators
{
    public class ReservationDtoValidator : AbstractValidator<ReservationDto>
    {
        public ReservationDtoValidator()
        {
            RuleFor(r => r.StartDate).NotEmpty().WithMessage("Start date is required")
                .GreaterThan(System.DateTime.Now)
                .WithMessage("Start date must be greater than today");
            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("{PropertyName} is required")
                .GreaterThan(x => x.StartDate).WithMessage("{PropertyName} must be greater than Start Date");
        }
    }
}
