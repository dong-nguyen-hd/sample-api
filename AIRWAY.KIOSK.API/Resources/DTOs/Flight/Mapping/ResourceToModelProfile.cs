using AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request;
using AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip.Response;
using AIRWAY.KIOSK.API.Resources.Exceptions;

namespace AIRWAY.KIOSK.API.Resources.DTOs.Flight.Mapping;

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
            .ForMember(x => x.Type, opt => opt.MapFrom((src, dest, destMember, context) => context.State))
            .ForMember(x => x.Active, opt => opt.MapFrom(src => true))
            .ForMember(x => x.CreatedDatetimeUtc, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(x => x.UpdatedDatetimeUtc, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        #endregion

        #region Order info

        CreateMap<OrderInfoPassengerResponse, Model.Passenger>()
            .ForMember(x => x.BirthDay, opt => opt.MapFrom((src, dest) => ConvertDatetimeToDateonly(src.Birthday?.Date)))
            .ForMember(x => x.Type, opt => opt.MapFrom(src => ConvertPassengerType(src.Type)))
            .ForMember(x => x.Active, opt => opt.MapFrom(src => true))
            .ForMember(x => x.CreatedDatetimeUtc, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(x => x.UpdatedDatetimeUtc, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<AdditionalServiceRequest, Model.AdditionalService>()
            .ForMember(x => x.Type, opt => opt.MapFrom((src, dest, destMember, context) => context.State))
            .ForMember(x => x.Active, opt => opt.MapFrom(src => true))
            .ForMember(x => x.CreatedDatetimeUtc, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(x => x.UpdatedDatetimeUtc, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));
        
        CreateMap<GetBaggageInnerResponse, Model.AdditionalService>()
            .ForMember(x => x.Type, opt => opt.MapFrom((src, dest, destMember, context) => context.State))
            .ForMember(x => x.Active, opt => opt.MapFrom(src => true))
            .ForMember(x => x.CreatedDatetimeUtc, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(x => x.UpdatedDatetimeUtc, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        #endregion
    }

    #region Private work

    private static DateOnly? ConvertDatetimeToDateonly(DateTime? source) =>
        source != null ? DateOnly.FromDateTime(source.Value) : null;

    private static MyEnum.PassengerType ConvertPassengerType(string? source)
    {
        if (string.IsNullOrEmpty(source))
            throw new MessageResultException("Dữ liệu passenger không hợp lệ");

        if (source.Equals("ADT", StringComparison.OrdinalIgnoreCase))
            return MyEnum.PassengerType.ADT;
        if (source.Equals("CHD", StringComparison.OrdinalIgnoreCase))
            return MyEnum.PassengerType.CHD;
        if (source.Equals("INF", StringComparison.OrdinalIgnoreCase))
            return MyEnum.PassengerType.INF;
        
        throw new MessageResultException("Dữ liệu passenger không hợp lệ");
    }

    #endregion
}