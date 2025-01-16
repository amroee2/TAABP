using FluentValidation;
using TAABP.Application.DTOs;

namespace TAABP.Application.Validators
{
    public class CityValidator : AbstractValidator<CityDto>
    {
        public CityValidator()
        {
            RuleFor(x=>x.Name).NotEmpty().WithMessage("City name is required");
            RuleFor(x => x.Name).MaximumLength(30).WithMessage("City name cannot exceed 100 characters");
            RuleFor(x=>x.Description).NotEmpty().WithMessage("City description is required");
            RuleFor(x => x.Description).MaximumLength(500).WithMessage("City description cannot exceed 500 characters");
            RuleFor(x => x.Country).NotEmpty().WithMessage("Country is required");
            RuleFor(x => x.Country).MaximumLength(30).WithMessage("Country cannot exceed 30 characters");
            RuleFor(x => x.PostOffice).NotEmpty().WithMessage("Post office is required");
            RuleFor(x => x.PostOffice).MaximumLength(30).WithMessage("Post office cannot exceed 30 characters");
            RuleFor(x => x.Thumbnail).NotEmpty().WithMessage("Thumbnail is required");
        }
    }
}
