using FluentValidation;
using TAABP.Application.DTOs;

namespace TAABP.Application.Validators
{
    public class HotelDtoValidator : AbstractValidator<HotelDto>
    {
        public HotelDtoValidator()
        {
            RuleFor(h => h.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(h => h.Address).NotEmpty().WithMessage("Address is required");
            RuleFor(h => h.PhoneNumber).NotEmpty().WithMessage("Phone is required");
            RuleFor(h => h.Rating).NotEmpty().WithMessage("Rating is required");
            RuleFor(h => h.Rating).InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");
            RuleFor(h => h.NumberOfRooms).NotEmpty().WithMessage("Number of rooms is required");
            RuleFor(h => h.NumberOfRooms).GreaterThan(0).WithMessage("Number of rooms must be greater than 0");
        }
    }
}
