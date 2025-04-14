using AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.IACV.Request;
using FluentValidation;

namespace AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.IACV.Validation;

public class QueryDataValidator : AbstractValidator<QueryDataRequest>
{
    public QueryDataValidator()
    {
        RuleFor(x => x.Filter)
            .NotEmpty()
            .NotNull()
            .Must(ValidateFilter)
            .When(x => x.Filter != null);

        RuleFor(x => x.PartnerKey)
            .NotEmpty()
            .NotNull()
            .Must(x => !string.IsNullOrEmpty(x) && x.Length is > 0 and <= 250);

        RuleFor(x => x.PlatformType)
            .NotEmpty()
            .NotNull()
            .Must(x => x != null && Enum.IsDefined(typeof(MyEnum.PlatformType), x));
    }

    private static bool ValidateFilter(SearchRequest? source)
    {
        // Validate abtrip-order-id
        if (!string.IsNullOrEmpty(source?.OrderId))
            if (source.OrderId.Length > 250)
                return false;

        // Validate point-name
        if (!string.IsNullOrEmpty(source?.PointName))
            if (source.PointName.Length > 250)
                return false;

        // Validate start-date, end-date
        DateTime tempNow = DateTime.UtcNow.ConvertUtcToVietnamTz();
        DateOnly now = DateOnly.FromDateTime(tempNow);

        if (source?.StartDate != null)
            if (source.StartDate.Value > now)
                return false;

        if (source?.EndDate != null)
            if (source.EndDate.Value > now)
                return false;

        if (source?.StartDate != null && source?.EndDate != null)
            if (source.StartDate.Value > source.EndDate.Value)
                return false;

        return true;
    }
}