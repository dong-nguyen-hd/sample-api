using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request;
using FluentValidation;

namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Validation;

public class SearchValidator : AbstractValidator<SearchRequest>
{
    public SearchValidator()
    {
        RuleFor(x => x.Adt)
            .NotEmpty()
            .NotNull()
            .Must((x, y) => y > 0);

        RuleFor(x => x.Chd)
            .Must((x, y) => y >= 0 && x.Adt + y <= 9)
            .When(x => x.Chd != null);

        RuleFor(x => x.Inf)
            .Must((x, y) => x.Adt == y)
            .When(x => x.Inf != null && x.Inf > 0);

        RuleFor(x => x.ListFlight)
            .NotNull()
            .NotEmpty()
            .Must(ValidateListFlight);
    }

    #region List Flight Validate

    private static bool ValidateListFlight(List<SearchFlightRequest>? source)
    {
        if (source is { Count: <= 0 })
            return false;

        var localTime = DateTime.UtcNow.ConvertUtcToVietnamTz();
        foreach (var flight in source)
            if (string.IsNullOrEmpty(flight.StartPoint) ||
                string.IsNullOrEmpty(flight.EndPoint) ||
                flight.DepartDate == null ||
                (flight.DepartDate.Value.Date - localTime.Date).Days < 0)
                return false;

        if (source.Count == 2)
        {
            if (source[0].StartPoint == source[1].EndPoint && source[0].EndPoint == source[1].StartPoint)
                return true;

            return false;
        }

        return true;
    }

    #endregion
}