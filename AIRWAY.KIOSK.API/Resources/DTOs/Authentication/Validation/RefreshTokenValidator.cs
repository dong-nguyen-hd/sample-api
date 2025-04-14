using AIRWAY.KIOSK.API.Resources.DTOs.Authentication.Request;
using FluentValidation;

namespace AIRWAY.KIOSK.API.Resources.DTOs.Authentication.Validation;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .NotNull();
    }
}