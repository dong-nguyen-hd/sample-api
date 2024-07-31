namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Account.Mapping;

using Request;

public sealed class ResourceToModelProfile : Profile
{
    public ResourceToModelProfile()
    {
        var utcNow = DateTime.UtcNow;

        CreateMap<CreateRequest, Model.Account>()
            .ForMember(x => x.Email, opt => opt.MapFrom(src => src.Email.RemoveAllSpaceChar()))
            .ForMember(x => x.Name, opt => opt.MapFrom(src => src.Name.ToLowerAndRemoveSpace()))
            .ForMember(x => x.UserName, opt => opt.MapFrom(src => src.UserName.ToLowerAndRemoveSpace()))
            .ForMember(x => x.Password, opt => opt.MapFrom(src => src.Password.HashingPassword(SystemConstant.IterationCount)))
            .ForMember(x => x.Active, opt => opt.MapFrom(src => true))
            .ForMember(x => x.CreatedDatetimeUtc, opt => opt.MapFrom(src => utcNow))
            .ForMember(x => x.UpdatedDatetimeUtc, opt => opt.MapFrom(src => utcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<UpdateRequest, Model.Account>()
            .ForMember(x => x.Email, opt => opt.MapFrom(src => src.Email.RemoveAllSpaceChar()))
            .ForMember(x => x.Name, opt => opt.MapFrom(src => src.Name.ToLowerAndRemoveSpace()))
            .ForMember(x => x.UserName, opt => opt.MapFrom(src => src.UserName.ToLowerAndRemoveSpace()))
            .ForMember(x => x.UpdatedDatetimeUtc, opt => opt.MapFrom(src => utcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));
    }
}
