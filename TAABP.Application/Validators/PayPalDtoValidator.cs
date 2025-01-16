using FluentValidation;
using TAABP.Application.DTOs;

namespace TAABP.Application.Validators
{
    public class PayPalDtoValidator : AbstractValidator<PayPalDto>
    {

        public PayPalDtoValidator()
        {
            RuleFor(x => x.PayPalEmail)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email is not valid");
        }
    }
}
