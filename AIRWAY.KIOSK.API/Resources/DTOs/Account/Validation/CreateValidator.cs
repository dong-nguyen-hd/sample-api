using System.Buffers.Text;
using AIRWAY.KIOSK.API.Resources.DTOs.Account.Request;
using FluentValidation;

namespace AIRWAY.KIOSK.API.Resources.DTOs.Account.Validation;

public class CreateValidator : AbstractValidator<CreateRequest>
{
    public CreateValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().NotNull().Must(x => x.Length <= 150);
        RuleFor(x => x.Password).NotEmpty().NotNull().Must(x => x.Length >= 6 && Base64.IsValid(x));
        RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
        RuleFor(x => x.SystemRoles).Must(x => x.TrueForAll(MyPolicy.IsValid));
    }
}