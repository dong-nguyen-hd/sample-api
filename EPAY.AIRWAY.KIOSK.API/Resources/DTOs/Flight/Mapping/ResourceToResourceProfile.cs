using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;
using Microsoft.OpenApi.Extensions;
using AbTrip = EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip;

namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Mapping;

public sealed class ResourceToResourceProfile : Profile
{
    public ResourceToResourceProfile()
    {
        #region Fare and Flight

        CreateMap<SearchRequest, AbTrip.Request.SearchFlightRequest>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<SearchFlightRequest, AbTrip.Request.SearchFlightInner>()
            .ForMember(x => x.DepartDate, opt => opt.MapFrom(src => ConvertToDatetimeRaw(src.DepartDate)))
            .ForMember(x => x.Airline, opt => opt.MapFrom(src => src.Airline))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<AbTrip.Response.SearchFlightResponse, SearchResponse>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<AbTrip.Response.FareRuleResponse, FareRulesResponse>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<AbTrip.Response.RulesGroupResponse, RulesGroupResponse>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        #endregion

        #region Additional Services

        CreateMap<AdditionalServicesRequest, AbTrip.Request.GetAncillaryRequest>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<AdditionalServicesRequest, AbTrip.Request.GetBaggageRequest>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<FareDataRequest, AbTrip.Request.FareDataRequest>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<FlightRequest, AbTrip.Request.FlightRequest>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        #endregion

        #region Master data

        CreateMap<AbTrip.Response.AircraftsResponse, AircraftResponse>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<AbTrip.Response.AirlinesResponse, AirlineResponse>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<AbTrip.Response.AirportsResponse, AirportResponse>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        #endregion

        #region Verify

        CreateMap<VerifyRequest, AbTrip.Request.VerifyFlightRequest>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        #endregion

        #region Booking

        CreateMap<BookingRequest, AbTrip.Request.VerifyFlightRequest>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<InvoiceRequest, AbTrip.Request.Invoice>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<ContactRequest, AbTrip.Request.ContactRequest>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<BookingRequest, AbTrip.Request.BookFlightRequest>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<PassengerRequest, AbTrip.Request.PassengerRequest>()
            .ForMember(x => x.Birthday, opt => opt.MapFrom(src => ConvertToDatetimeRaw(src.Birthday)))
            .ForMember(x => x.ListBaggage, opt => opt.MapFrom(src => src.ListBaggage))
            .ForMember(x => x.ListService, opt => opt.MapFrom(src => src.ListService))
            .ForMember(x => x.Type, opt => opt.MapFrom(src => Enum.GetName(src.Type)))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<AdditionalServiceRequest, AbTrip.Request.AdditionalServiceRequest>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<AbTrip.Response.BookingPassengerResponse, PassengerResponse>()
            .ForMember(x => x.Type, opt => opt.MapFrom(src => ConvertPassengerType(src.Type)))
            .ForMember(x => x.Birthday, opt => opt.MapFrom(src => src.Birthday.ConvertStringToDatetime("ddMMyyyy")))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<AbTrip.Response.GetAncillaryInnerResponse, AncillaryResponse>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<AbTrip.Response.GetBaggageResponse, BaggageResponse>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));
        
        CreateMap<AbTrip.Response.GetBaggageInnerResponse, BaggageResponse>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));
        
        CreateMap<AbTrip.Response.GetAncillaryInnerResponse, AncillaryResponse>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        #endregion
    }

    #region Private work

    private static string? ConvertToDatetimeRaw(DateTime? dateTime) => dateTime?.ToString("ddMMyyyy");

    private static MyEnum.PassengerType? ConvertPassengerType(string? source) => source?.ToUpper() switch
    {
        "ADT" => MyEnum.PassengerType.ADT,
        "CHD" => MyEnum.PassengerType.CHD,
        "INF" => MyEnum.PassengerType.INF,
        _ => null
    };

    #endregion
}