using AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request;
using FluentValidation;

namespace AIRWAY.KIOSK.API.Resources.DTOs.Flight.Validation;

public class CheckOrderInfoValidator : AbstractValidator<CheckOrderInfoRequest>
{
    public CheckOrderInfoValidator()
    {
        RuleFor(x => x.AbTripOrderId)
            .NotNull()
            .NotEmpty()
            .Must(x => !string.IsNullOrEmpty(x) && x.Length <= 50);
    }
}