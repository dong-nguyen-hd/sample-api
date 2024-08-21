using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request;

namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Mapping;

public class ResourceToModelProfile : Profile
{
    public ResourceToModelProfile()
    {
        #region Booking

        CreateMap<InvoiceRequest, Model.Invoice>()
            .ForMember(x => x.Active, opt => opt.MapFrom(src => true))
            .ForMember(x => x.CreatedDatetimeUtc, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(x => x.UpdatedDatetimeUtc, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<ContactRequest, Model.Contact>()
            .ForMember(x => x.Active, opt => opt.MapFrom(src => true))
            .ForMember(x => x.CreatedDatetimeUtc, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(x => x.UpdatedDatetimeUtc, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<PassengerRequest, Model.Passenger>()
            .ForMember(x => x.BirthDay, opt => opt.MapFrom(src => ConvertDatetimeToDateonly(src.Birthday)))
            .ForMember(x => x.Active, opt => opt.MapFrom(src => true))
            .ForMember(x => x.CreatedDatetimeUtc, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(x => x.UpdatedDatetimeUtc, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<AdditionalServiceRequest, Model.AdditionalService>()
            .ForMember(x => x.Active, opt => opt.MapFrom(src => true))
            .ForMember(x => x.CreatedDatetimeUtc, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(x => x.UpdatedDatetimeUtc, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        #endregion
    }

    #region Private work

    private static DateOnly? ConvertDatetimeToDateonly(DateTime? source) =>
        source != null ? DateOnly.FromDateTime(source.Value) : null;

    #endregion
}