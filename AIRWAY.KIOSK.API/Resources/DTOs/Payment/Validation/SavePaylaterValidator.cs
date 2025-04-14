using AIRWAY.KIOSK.API.Resources.DTOs.Payment.Request;
using FluentValidation;

namespace AIRWAY.KIOSK.API.Resources.DTOs.Payment.Validation;

public sealed class SavePaylaterValidator : AbstractValidator<SavePaylaterRequest>
{
    public SavePaylaterValidator()
    {
        RuleFor(x => x.BillId)
            .NotEmpty()
            .NotNull()
            .Must(x => x?.Length <= 50);
        
        RuleFor(x => x.PlatformType)
            .NotEmpty()
            .NotNull()
            .Must(x => Enum.IsDefined(typeof(MyEnum.PlatformType), x));
    }
}