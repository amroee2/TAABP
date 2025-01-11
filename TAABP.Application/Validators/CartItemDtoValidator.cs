using FluentValidation;
using TAABP.Application.DTOs.ShoppingDto;

namespace TAABP.Application.Validators
{
    public class CartItemDtoValidator : AbstractValidator<CartItemDto>
    {
        public CartItemDtoValidator()
        {
            RuleFor(x => x.StartDate)
                .NotEmpty()
                .GreaterThan(DateTime.Now);
            RuleFor(x => x.EndDate)
                .NotEmpty()
                .WithMessage("End date is required")
                .GreaterThan(x=>x.StartDate);

        }
    }
}
