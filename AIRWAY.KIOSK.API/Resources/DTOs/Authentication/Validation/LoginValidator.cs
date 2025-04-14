using System.Buffers.Text;
using AIRWAY.KIOSK.API.Resources.DTOs.Authentication.Request;
using FluentValidation;

namespace AIRWAY.KIOSK.API.Resources.DTOs.Authentication.Validation;

public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .NotNull()
            .Must(x => x.Length <= 150);

        RuleFor(x => x.Password)
            .NotEmpty()
            .NotNull()
            .Must(x => x.Length <= 150);

        RuleFor(x => x.Type)
            .NotEmpty()
            .NotNull()
            .Must(x => Enum.IsDefined(x!.Value))
            .When(x => x.Type != null);
    }
}