using AIRWAY.KIOSK.API.Resources.DTOs.Device.Response;

namespace AIRWAY.KIOSK.API.Resources.DTOs.Device.Mapping;

public class ModelToResourceProfile : Profile
{
    public ModelToResourceProfile()
    {
        CreateMap<Model.ReportSection.Device, DeviceResponse>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));
    }
}