namespace AIRWAY.KIOSK.API.Resources.DTOs.Authentication.Mapping;

using Response;

public sealed class ModelToResourceProfile : Profile
{
    public ModelToResourceProfile()
    {
        CreateMap<Model.RefreshToken, TokenResponse>()
            .ForMember(x => x.RefreshTokenExpireTimeUTC, opt => opt.MapFrom(src => src.ExpiredUtc))
            .ForMember(x => x.RefreshToken, opt => opt.MapFrom(src => src.Token))
            .ForMember(x => x.AccessToken, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));
    }
}
