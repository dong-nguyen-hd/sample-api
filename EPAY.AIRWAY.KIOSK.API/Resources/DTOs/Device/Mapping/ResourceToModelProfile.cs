using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Device.Request;

namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Device.Mapping;

public class ResourceToModelProfile : Profile
{
    public ResourceToModelProfile()
    {
        CreateMap<CreateRequest, Model.Device>()
            .ForMember(x => x.Active, opt => opt.MapFrom(src => true))
            .ForMember(x => x.CreatedDatetimeUtc, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(x => x.UpdatedDatetimeUtc, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));
        
        CreateMap<UpdateRequest, Model.Device>()
            .ForMember(x => x.UpdatedDatetimeUtc, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));
    }
}