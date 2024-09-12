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
            .ForMember(x => x.TicketIssueStatus, opt => opt.MapFrom(src => ConvertPaymentStatus(src)))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));
    }

    #region Private work

    private static TicketIssueStatus ConvertPaymentStatus(Model.PaymentTransaction paymentTransaction)
    {
        if (paymentTransaction.PaymentProviderStatus == PaymentStatus.Success)
        {
            if (paymentTransaction.ServiceProviderStatus == ServiceStatus.Success)
                return TicketIssueStatus.Success;
            if (paymentTransaction.ServiceProviderStatus == ServiceStatus.HalfSuccess)
                return TicketIssueStatus.HalfSuccess;
            
            return TicketIssueStatus.Fail;
        }
            
        return TicketIssueStatus.Fail;
    }

    #endregion
}