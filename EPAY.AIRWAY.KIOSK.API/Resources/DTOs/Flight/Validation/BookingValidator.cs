using System.Text.RegularExpressions;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request;
using FluentValidation;

namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Validation;

public class BookingValidator : AbstractValidator<BookingRequest>
{
    public BookingValidator()
    {
        RuleFor(x => x.Contact).Must(ValidateContact);

        RuleFor(x => x.Invoice).Must(ValidateInvoice);

        RuleFor(x => x.ListPassenger).Must(ValidatePassenger);

        RuleFor(x => x.ListFareData)
            .NotNull()
            .NotEmpty()
            .Must(ValidateListFare);
    }

    private static bool ValidateContact(ContactRequest? source)
    {
        if (source == null)
            return false;
        if (string.IsNullOrEmpty(source.FirstName))
            return false;
        if (string.IsNullOrEmpty(source.LastName))
            return false;
        if (string.IsNullOrEmpty(source.Phone) || !Regex.IsMatch(source.Phone, @"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}"))
            return false;
        if (string.IsNullOrEmpty(source.Email) || !Regex.IsMatch(source.Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
            return false;

        return true;
    }

    private static bool ValidateInvoice(InvoiceRequest? source)
    {
        if (source == null)
            return true;

        if (string.IsNullOrEmpty(source.TaxCode))
            return false;
        if (string.IsNullOrEmpty(source.CompanyNameReceive))
            return false;
        if (string.IsNullOrEmpty(source.AddressReceive))
            return false;
        if (string.IsNullOrEmpty(source.CityNameReceive))
            return false;
        if (string.IsNullOrEmpty(source.ReceiverReceive))
            return false;

        return true;
    }

    private static bool ValidatePassenger(List<PassengerRequest>? source)
    {
        var localTime = DateTime.UtcNow.ConvertUtcToVietnamTz();

        if (source is { Count: <= 0 })
            return false;
        foreach (var passenger in source!)
        {
            if (string.IsNullOrEmpty(passenger.FirstName))
                return false;
            if (string.IsNullOrEmpty(passenger.LastName))
                return false;
            if (!Enum.IsDefined(passenger.Type))
                return false;

            if (passenger is { Type: MyEnum.PassengerType.ADT, Birthday: not null })
            {
                var age = localTime.Date.Year - passenger.Birthday.Value.Year;
                if (age < 12)
                    return false;
            }

            if (passenger.Type == MyEnum.PassengerType.CHD)
            {
                if (passenger.Birthday == null)
                    return false;

                var age = localTime.Date.Year - passenger.Birthday.Value.Year;
                if (age is > 12 or < 2)
                    return false;
            }

            if (passenger.Type == MyEnum.PassengerType.INF)
            {
                if (passenger.Birthday == null)
                    return false;

                var age = localTime.Date.Year - passenger.Birthday.Value.Year;
                if (age is < 0 or > 2)
                    return false;
            }

            // Validate baggage
            if (passenger.ListBaggage is { Count: > 0 })
            {
                foreach (var baggage in passenger.ListBaggage)
                {
                    if (baggage.Price <= 0)
                        return false;
                    if (string.IsNullOrEmpty(baggage.Code))
                        return false;
                }
            }

            // Validate service
            if (passenger.ListService is { Count: > 0 })
            {
                foreach (var service in passenger.ListService)
                {
                    if (service.Price <= 0)
                        return false;
                    if (string.IsNullOrEmpty(service.Code))
                        return false;
                }
            }
        }

        return true;
    }

    private static bool ValidateListFare(List<FareDataRequest>? source)
    {
        if (source is { Count: <= 0 })
            return false;

        foreach (var fare in source!)
        {
            if (string.IsNullOrEmpty(fare.Session))
                return false;
            if (fare.FareDataId == null)
                return false;

            if (fare.ListFlight is { Count: <= 0 })
                return false;
            foreach (var flight in fare.ListFlight!)
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