using FluentValidation;
using TAABP.Application.DTOs;
using TAABP.Core;

namespace TAABP.Application.Validators
{
    public class FilterOptionsDtoValidator : AbstractValidator<FilterOptionsDto>
    {
        public FilterOptionsDtoValidator()
        {
            RuleFor(x=>x.PriceRange).NotNull().NotEmpty().WithMessage("Price range is required")
                .ChildRules(x => x.RuleFor(y => y[0]).NotNull().NotEmpty().WithMessage("Minimum price is required")
                .GreaterThan(0).WithMessage("Minimum price must be greater than 0")
                .LessThan(y => y[1]).WithMessage("Minimum price must be less than maximum price"))
                .ChildRules(x => x.RuleFor(y => y[1]).NotNull().NotEmpty().WithMessage("Maximum price is required")
                .GreaterThan(0).WithMessage("Maximum price must be greater than 0"));
            RuleFor(x => x.StarRating).NotNull().NotEmpty().WithMessage("Star rating is required")
                .ForEach(x => x.InclusiveBetween(1, 5).WithMessage("Star rating must be between 1 and 5"));
            RuleFor(x => x.Amenities).NotNull().NotEmpty().WithMessage("Amenities are required");
            RuleFor(x => x.RoomType)
                  .NotEmpty().WithMessage("Please enter the room type")
                  .Must(type => Enum.TryParse(typeof(RoomType), type.ToString(), true, out _))
                  .WithMessage($"Invalid room type. Allowed types: {string.Join(", ", Enum.GetNames(typeof(RoomType)))}");
        }
    }
}
