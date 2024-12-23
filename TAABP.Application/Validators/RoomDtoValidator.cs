using FluentValidation;
using System.Linq;
using TAABP.Application.DTOs;
using TAABP.Core;

namespace TAABP.Application.Validators
{ 
    public class RoomDtoValidator : AbstractValidator<RoomDto>
    {
        public RoomDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Please enter the room name")
                .MaximumLength(30);
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Please enter the room description")
                .MaximumLength(500);
            RuleFor(x => x.Thumbnail)
                .NotEmpty().WithMessage("Please enter the room thumbnail")
                .MaximumLength(100);
            RuleFor(x => x.AdultsCapacity)
                .NotEmpty().WithMessage("Please enter the room adults capacity")
                .GreaterThanOrEqualTo(0).WithMessage("Adult capacity must be greater than or equal to 0");
            RuleFor(x => x.ChildrenCapacity)
                .NotEmpty().WithMessage("Please enter the room children capacity")
                .GreaterThanOrEqualTo(0).WithMessage("Children capacity must be greater than or equal to 0");
            RuleFor(x => x.PricePerNight)
                .NotEmpty()
                .WithMessage("Please enter the room price per night")
                .GreaterThan(0)
                .WithMessage("Price per night must be greater than 0");
            RuleFor(x => x.IsAvailable)
                .NotEmpty().WithMessage("Please enter the room status");
            RuleFor(x => x.Number)
                .NotEmpty().WithMessage("Please enter the room number")
                .GreaterThan(0).WithMessage("Room number must be greater than 0");
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Please enter the room type")
                .Must(type => Enum.TryParse(typeof(RoomType), type.ToString(), true, out _))
                .WithMessage($"Invalid room type. Allowed types: {string.Join(", ", Enum.GetNames(typeof(RoomType)))}");
        }
    }
}
