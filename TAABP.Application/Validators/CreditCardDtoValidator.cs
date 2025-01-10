using FluentValidation;
using TAABP.Application.DTOs;

namespace TAABP.Application.Validators
{
    public class CreditCardDtoValidator : AbstractValidator<CreditCardDto>
    {
        public CreditCardDtoValidator()
        {
            RuleFor(c => c.CardNumber)
                .NotEmpty().WithMessage("Card number is required.")
                .CreditCard().WithMessage("Invalid card number.");

            RuleFor(c => c.CardHolderName)
                .NotEmpty().WithMessage("Card holder name is required.");

            RuleFor(c => c.CVV)
                .NotEmpty().WithMessage("CVV is required.")
                .Length(3).WithMessage("Invalid CVV.");

            RuleFor(c => c.ExpirationDate)
                .NotEmpty().WithMessage("Expiration date is required.")
                .GreaterThan(System.DateTime.Now).WithMessage("Invalid expiration date.");
        }
    }
}
