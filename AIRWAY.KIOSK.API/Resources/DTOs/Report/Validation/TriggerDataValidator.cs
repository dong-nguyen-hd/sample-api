using AIRWAY.KIOSK.API.Resources.DTOs.Report.Request;
using FluentValidation;

namespace AIRWAY.KIOSK.API.Resources.DTOs.Report.Validation;

public class TriggerDataValidator : AbstractValidator<TriggerDataRequest>
{
    public TriggerDataValidator()
    {
        RuleFor(x => x.Date).Must(ValidateDate);
    }

    private static bool ValidateDate(DateOnly source)
    {
        DateTime now = DateTime.UtcNow.ConvertUtcToVietnamTz();

        return now.Date.CompareTo(source) > 0;
    }
}