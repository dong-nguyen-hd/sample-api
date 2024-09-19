using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;

namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Mapping;

public class ModelToResourceProfile : Profile
{
    public ModelToResourceProfile()
    {
        #region Booking

        CreateMap<Model.Bill, BookingResponse>()
            .ForMember(x => x.BillId, opt => opt.MapFrom(src => src.Id))
            .ForMember(x => x.ExpiryDate, opt => opt.MapFrom(src => src.ExpiredDatetimeUtc.ConvertUtcToVietnamTz()))
            .ForMember(x => x.ListPassenger, opt => opt.MapFrom(src => src.Passengers))
            .ForMember(x => x.ListFareData, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<Model.Passenger, PassengerResponse>()
            .ForMember(x => x.ListBaggage, opt => opt.MapFrom(src => MappingAdditionalService(src.AdditionalServices, MyEnum.AdditionalServiceType.Baggage)))
            .ForMember(x => x.ListService, opt => opt.MapFrom(src => MappingAdditionalService(src.AdditionalServices, MyEnum.AdditionalServiceType.Service)))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));
        
        CreateMap<Model.AdditionalService, BaggageResponse>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));
        
        CreateMap<Model.AdditionalService, AncillaryResponse>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        #endregion

        #region Order info
        
        CreateMap<PassengerResponse, Model.Passenger>()
            .ForMember(x => x.BirthDay, opt => opt.MapFrom(src => ConvertDatetimeToDateonly(src.Birthday)))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<Model.AdditionalService, BaggageResponse>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<Model.AdditionalService, AncillaryResponse>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        #endregion
    }

    #region Private work

    private List<Model.AdditionalService>? MappingAdditionalService(HashSet<Model.AdditionalService>? source, MyEnum.AdditionalServiceType type)
    {
        if (source == null || source.Count <= 0)
            return null;

        return source.Where(x => x.Type == type).ToList();
    }
    
    private static DateOnly? ConvertDatetimeToDateonly(DateTime? source) =>
        source != null ? DateOnly.FromDateTime(source.Value) : null;

    #endregion
}