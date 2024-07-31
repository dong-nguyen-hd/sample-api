using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Payment.Response;
using EPAY.AIRWAY.KIOSK.API.Resources.Enums;

namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Payment.Mapping;

public sealed class ModelToResourceProfile : Profile
{
    public ModelToResourceProfile()
    {
        CreateMap<Model.PaymentTransaction, GenerateResponse>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<Model.PaymentTransaction, CheckResponse>()
            .ForMember(x => x.PaymentStatus, opt => opt.MapFrom(src => ConvertPaymentStatus(src)))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));
    }

    #region Private work

    private static PaymentStatus ConvertPaymentStatus(Model.PaymentTransaction paymentTransaction)
    {
        if (paymentTransaction is { PaymentProviderStatus: PaymentStatus.Success, ServiceProviderStatus: PaymentStatus.Success })
            return PaymentStatus.Success;

        return PaymentStatus.Fail;
    }

    #endregion
}