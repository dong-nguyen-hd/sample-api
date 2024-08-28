using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Device.Response;

namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Device.Mapping;

public class ModelToResourceProfile : Profile
{
    public ModelToResourceProfile()
    {
        CreateMap<Model.Device, DeviceResponse>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));
    }
}