namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Account.Mapping;

using Response;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Authentication.Response;

public sealed class ModelToResourceProfile : Profile
{
    public ModelToResourceProfile()
    {
        CreateMap<Model.Account, AccountResponse>()
            .ForMember(x => x.CreatedDatetime, opt => opt.MapFrom(src => src.CreatedDatetimeUtc.ConvertUtcToVietnamTz()))
            .ForMember(x => x.UpdatedDatetime, opt => opt.MapFrom(src => src.UpdatedDatetimeUtc.ConvertUtcToVietnamTz()))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<Model.Account, AccessTokenResponse>()
            .ForMember(x => x.CreatedDatetime, opt => opt.MapFrom(src => src.CreatedDatetimeUtc.ConvertUtcToVietnamTz()))
            .ForMember(x => x.UpdatedDatetime, opt => opt.MapFrom(src => src.UpdatedDatetimeUtc.ConvertUtcToVietnamTz()))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));
    }
}