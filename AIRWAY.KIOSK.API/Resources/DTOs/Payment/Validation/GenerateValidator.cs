using AIRWAY.KIOSK.API.Resources.DTOs.Payment.Request;
using AIRWAY.KIOSK.API.Resources.Enums;
using FluentValidation;

namespace AIRWAY.KIOSK.API.Resources.DTOs.Payment.Validation;

public sealed class GenerateValidator : AbstractValidator<GenerateRequest>
{
    public GenerateValidator()
    {
        RuleFor(x => x.PaymentType)
            .NotEmpty()
            .NotNull()
            .Must(ValidatePaymentType);

        RuleFor(x => x.BillId)
            .NotEmpty()
            .NotNull()
            .Must(x => !string.IsNullOrEmpty(x) && x.Length <= 50);

        RuleFor(x => x.PlatformType)
            .NotEmpty()
            .NotNull()
            .Must(x => Enum.IsDefined(typeof(PlatformType), x));

        RuleFor(x => x.ReturnUrl)
            .NotEmpty()
            .NotNull()
            .Must(x => x?.Length <= 500)
            .When(x => !string.IsNullOrEmpty(x.ReturnUrl));

        RuleFor(x => x.Customer)
            .Must(ValidateCustomer)
            .When(x => x.PaymentType == PaymentType.EpayWallet && x.PlatformType == PlatformType.Vneid);
    }

    private static bool ValidatePaymentType(MyEnum.PaymentType paymentType)
    {
        if (!Enum.IsDefined(typeof(PaymentType), paymentType))
            return false;
        if (paymentType == PaymentType.PayLater) // Không hỗ trợ phương thức thanh toán này.
            return false;

        return true;
    }

    private static bool ValidateCustomer(CustomerRequest? source)
    {
        if (source == null)
            return false;
        if (string.IsNullOrEmpty(source.IdNumber))
            return false;

        return true;
    }
}