namespace AIRWAY.KIOSK.API.Resources.DTOs.Configuration.Mapping;

using Response;

public sealed class ModelToResourceProfile : Profile
{
    public ModelToResourceProfile()
    {
        CreateMap<Model.Configuration, ConfigurationResponse>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));
    }
}
