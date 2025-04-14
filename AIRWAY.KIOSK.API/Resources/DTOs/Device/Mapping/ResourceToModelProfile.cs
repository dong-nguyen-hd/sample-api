using AIRWAY.KIOSK.API.Resources.DTOs.Device.Request;

namespace AIRWAY.KIOSK.API.Resources.DTOs.Device.Mapping;

public class ResourceToModelProfile : Profile
{
    public ResourceToModelProfile()
    {
        CreateMap<CreateRequest, Model.ReportSection.Device>()
            .ForMember(x => x.ServicePartnerId, opt => opt.MapFrom(src => src.ServicePartnerId))
            .ForMember(x => x.LocationId, opt => opt.MapFrom(src => src.LocationId))
            .ForMember(x => x.Active, opt => opt.MapFrom(src => true))
            .ForMember(x => x.CreatedDatetimeUtc, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(x => x.UpdatedDatetimeUtc, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));
        
        CreateMap<UpdateRequest, Model.ReportSection.Device>()
            .ForMember(x => x.UpdatedDatetimeUtc, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));
    }
}