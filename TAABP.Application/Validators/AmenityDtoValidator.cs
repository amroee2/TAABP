using FluentValidation;
using TAABP.Application.DTOs;

namespace TAABP.Application.Validators
{
    public class AmenityDtoValidator : AbstractValidator<AmenityDto>
    {
        public AmenityDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name can't be empty");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description can't be empty");
            RuleFor(x => x.ImageUrl).NotEmpty().WithMessage("Image is required");
            RuleFor(x => x.HotelId).NotEmpty().WithMessage("Hotel id is required");
        }
    }
}
