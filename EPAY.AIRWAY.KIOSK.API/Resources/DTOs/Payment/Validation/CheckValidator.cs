using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Payment.Request;
using FluentValidation;

namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Payment.Validation;

public sealed class CheckValidator : AbstractValidator<CheckRequest>
{
    public CheckValidator()
    {
        RuleFor(x => x.OrderCode)
            .NotEmpty()
            .NotNull()
            .Must(x => x?.Length <= 50);
        
        RuleFor(x => x.BillId)
            .NotEmpty()
            .NotNull()
            .Must(x => x?.Length <= 50);
    }
}