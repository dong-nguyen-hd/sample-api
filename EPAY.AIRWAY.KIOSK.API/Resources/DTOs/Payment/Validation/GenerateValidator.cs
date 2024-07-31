using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Payment.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.Enums;
using FluentValidation;

namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Payment.Validation;

public sealed class GenerateValidator : AbstractValidator<GenerateRequest>
{
    public GenerateValidator()
    {
        RuleFor(x => x.PaymentType)
            .NotEmpty()
            .NotNull()
            .Must(x => Enum.IsDefined(typeof(PaymentType), x));
        
        RuleFor(x => x.BillCode).NotEmpty().NotNull().Must(x => x.Length <= 150);
        
        RuleFor(x => x.PosSerial)
            .NotEmpty()
            .NotNull()
            .Must(x => x.Length <= 150)
            .When(x => !string.IsNullOrEmpty(x.PosSerial));
        
        RuleFor(x => x.PosRefId)
            .NotEmpty()
            .NotNull()
            .Must(x => x.Length <= 150)
            .When(x => !string.IsNullOrEmpty(x.PosRefId));
        
        RuleFor(x => x.PosMerchantId)
            .NotEmpty()
            .NotNull()
            .Must(x => x.Length <= 150)
            .When(x => !string.IsNullOrEmpty(x.PosMerchantId));
        
        RuleFor(x => x.PosClientId)
            .NotEmpty()
            .NotNull()
            .Must(x => x.Length <= 150)
            .When(x => !string.IsNullOrEmpty(x.PosClientId));
        
        RuleFor(x => x.PosMerchantOutletId)
            .NotEmpty()
            .NotNull()
            .Must(x => x.Length <= 150)
            .When(x => !string.IsNullOrEmpty(x.PosMerchantOutletId));
        
        RuleFor(x => x.PosTerminalId)
            .NotEmpty()
            .NotNull()
            .Must(x => x.Length <= 150)
            .When(x => !string.IsNullOrEmpty(x.PosTerminalId));
        
        RuleFor(x => x.TotalAmount)
            .NotEmpty()
            .NotNull()
            .Must(x => x > 0);
        
        RuleFor(x => x.PlatformType)
            .NotEmpty()
            .NotNull()
            .Must(x => Enum.IsDefined(typeof(PlatformType), x));
        
        RuleFor(x => x.ReturnUrl)
            .NotEmpty()
            .NotNull()
            .Must(x => x.Length <= 250)
            .When(x => !string.IsNullOrEmpty(x.ReturnUrl));
        
        RuleFor(x => x.IdNumber)
            .NotEmpty()
            .NotNull()
            .Must(x => x.Length <= 50)
            .When(x => !string.IsNullOrEmpty(x.IdNumber));
    }
}