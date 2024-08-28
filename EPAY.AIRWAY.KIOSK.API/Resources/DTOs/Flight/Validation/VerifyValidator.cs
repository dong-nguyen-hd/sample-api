using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request;
using FluentValidation;

namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Validation;

public class VerifyValidator : AbstractValidator<VerifyRequest>
{
    public VerifyValidator()
    {
        RuleFor(x => x.TotalPrice)
            .NotNull()
            .NotEmpty()
            .Must(x => x > 0);

        RuleFor(x => x.ListFareData).Must(ValidateListFare);
    }

    private static bool ValidateListFare(List<FareDataRequest>? source)
    {
        if (source == null || source.Count <= 0)
            return false;

        foreach (var fare in source)
        {
            if (string.IsNullOrEmpty(fare.Session))
                return false;
            if (fare.FareDataId == null)
                return false;

            if (fare.ListFlight == null || fare.ListFlight.Count <= 0)
                return false;
            foreach (var flight in fare.ListFlight)
            {
                if (string.IsNullOrEmpty(flight.FlightValue))
                    return false;
                if (string.IsNullOrEmpty(flight.StartPoint))
                    return false;
                if (string.IsNullOrEmpty(flight.EndPoint))
                    return false;
            }
        }

        return true;
    }
}