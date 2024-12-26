using FluentValidation;
using TAABP.Application.DTOs;

namespace TAABP.Application.Validators
{
    public class ReservationValidator : AbstractValidator<ReservationDto>
    {
        public ReservationValidator()
        {
            RuleFor(r => r.StartDate).NotEmpty().WithMessage("Start date is required").GreaterThan(System.DateTime.Now).WithMessage("Start date must be greater than today");
            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("{PropertyName} is required")
                .GreaterThan(x => x.StartDate).WithMessage("{PropertyName} must be greater than Start Date");
            RuleFor(r => r.RoomId).NotEmpty().WithMessage("Room id is required");
            RuleFor(r => r.UserId).NotEmpty().WithMessage("User id is required");
        }
    }
}
