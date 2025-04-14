using AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request;
using FluentValidation;

namespace AIRWAY.KIOSK.API.Resources.DTOs.Flight.Validation;

public class AdditionalServicesValidator : AbstractValidator<AdditionalServicesRequest>
{
    public AdditionalServicesValidator()
    {
        RuleFor(x => x.ListFareData).Must(ValidateListFare);
    }

    #region List Fare Validate

    private static bool ValidateListFare(List<FareDataRequest?>? source)
    {
        if (source == null || source.Count <= 0)
            return false;

        foreach (var fare in source)
        {
            if (fare == null)
                return false;
            if (string.IsNullOrEmpty(fare.Session))
                return false;
            if (fare.FareDataId == null)
                return false;

            if (fare.ListFlight == null || fare.ListFlight.Count <= 0)
                return false;
            foreach (var flight in fare.ListFlight)
            {
                if (flight == null)
                    return false;
                if (string.IsNullOrEmpty(flight.FlightValue))
                    return false;
                if (string.IsNullOrEmpty(flight.StartPoint))
                    return false;
                if (string.IsNullOrEmpty(flight.EndPoint))
                    return false;
                if (flight.StartPoint.Equals(flight.EndPoint, StringComparison.OrdinalIgnoreCase))
                    return false;
            }
        }

        return true;
    }

    #endregion
}