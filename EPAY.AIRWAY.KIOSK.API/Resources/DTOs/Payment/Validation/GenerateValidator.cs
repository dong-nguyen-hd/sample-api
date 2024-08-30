using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Payment.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.Enums;
using FluentValidation;

namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Payment.Validation;

public sealed class GenerateValidator : AbstractValidator<GenerateRequest>
{
    public GenerateValidator()
    {
        RuleFor(x => x.PaymentType)
            .NotEmpty()
            .NotNull()
            .Must(x => Enum.IsDefined(typeof(PaymentType), x));

        RuleFor(x => x.BillId).NotEmpty().NotNull().Must(x => x.Length <= 150);

        RuleFor(x => x.TotalAmount)
            .NotEmpty()
            .NotNull()
            .Must(x => x > 0);

        RuleFor(x => x.PlatformType)
            .NotEmpty()
            .NotNull()
            .Must(x => Enum.IsDefined(typeof(PlatformType), x));

        RuleFor(x => x.ReturnUrl)
            .NotEmpty()
            .NotNull()
            .Must(x => x.Length <= 250)
            .When(x => !string.IsNullOrEmpty(x.ReturnUrl));
    }
}