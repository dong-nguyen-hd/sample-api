using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Authentication.Request;
using FluentValidation;

namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Authentication.Validation;

public class LogoutValidator : AbstractValidator<LogoutRequest>
{
    public LogoutValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .NotNull();
    }
}