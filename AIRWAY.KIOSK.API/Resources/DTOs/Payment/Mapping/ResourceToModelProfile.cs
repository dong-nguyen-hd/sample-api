using AIRWAY.KIOSK.API.Resources.DTOs.Payment.Request;

namespace AIRWAY.KIOSK.API.Resources.DTOs.Payment.Mapping;

public sealed class ResourceToModelProfile : Profile
{
    public ResourceToModelProfile()
    {
        var utcNow = DateTime.UtcNow;

        CreateMap<GenerateRequest, Model.PaymentTransaction>()
            .ForMember(x => x.Active, opt => opt.MapFrom(src => true))
            .ForMember(x => x.CreatedDatetimeUtc, opt => opt.MapFrom(src => utcNow))
            .ForMember(x => x.UpdatedDatetimeUtc, opt => opt.MapFrom(src => utcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));
    }
}