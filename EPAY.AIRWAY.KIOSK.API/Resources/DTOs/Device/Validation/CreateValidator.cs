using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Device.Request;
using FluentValidation;

namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Device.Validation;

public class CreateValidator : AbstractValidator<CreateRequest>
{
    public CreateValidator()
    {
        RuleFor(x => x.LocationId)
            .Must(x => x != null);

        RuleFor(x => x.ServicePartnerId)
            .Must(x => x != null);
    }
}