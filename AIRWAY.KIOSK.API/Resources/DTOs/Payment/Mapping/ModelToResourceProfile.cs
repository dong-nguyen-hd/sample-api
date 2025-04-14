using AIRWAY.KIOSK.API.Resources.DTOs.Payment.Response;
using AIRWAY.KIOSK.API.Resources.Enums;

namespace AIRWAY.KIOSK.API.Resources.DTOs.Payment.Mapping;

public sealed class ModelToResourceProfile : Profile
{
    public ModelToResourceProfile()
    {
        CreateMap<Model.PaymentTransaction, GenerateResponse>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<Model.PaymentTransaction, CheckResponse>()
            .ForMember(x => x.TicketIssueStatus, opt => opt.MapFrom(src => ConvertPaymentStatus(src)))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));
    }

    #region Private work

    private static TicketIssueStatus ConvertPaymentStatus(Model.PaymentTransaction paymentTransaction)
    {
        if (paymentTransaction.PaymentProviderStatus == PaymentStatus.Success)
            return TicketIssueStatus.Success;

        return TicketIssueStatus.Fail;
    }

    #endregion
}