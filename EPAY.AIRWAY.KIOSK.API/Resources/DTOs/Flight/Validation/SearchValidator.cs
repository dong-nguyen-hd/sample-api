using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request;
using FluentValidation;

namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Validation;


public class SearchValidator : AbstractValidator<SearchRequest>
{
    public SearchValidator()
    {
        RuleFor(x => x.Adt).NotEmpty().NotNull();
    }
}