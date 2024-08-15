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
            // Môi trường test, abTrip khuyến cáo chỉ search với chuyến bay mã VJ
            .ForMember(x => x.Airline, opt => opt.MapFrom(src => SystemGlobal.IsDebug ? "VJ" : src.Airline))
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

        CreateMap<AbTrip.Response.AircraftsResponse, AircraftsResponse>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<AbTrip.Response.AirlinesResponse, AirlinesResponse>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<AbTrip.Response.AirportsResponse, AirportsResponse>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        #endregion

        #region Booking

        // Verify
        CreateMap<BookingRequest, AbTrip.Request.VerifyFlightRequest>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<BookingFareRequest, AbTrip.Request.FareDataRequest>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<BookingFlightRequest, AbTrip.Request.FlightRequest>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        // Booking
        CreateMap<Invoice, AbTrip.Request.Invoice>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<ContactRequest, AbTrip.Request.ContactRequest>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<BookingRequest, AbTrip.Request.BookFlightRequest>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<PassengerRequest, AbTrip.Request.PassengerRequest>()
            .ForMember(x => x.ListBaggage, opt => opt.MapFrom(src => src.ListBaggage))
            .ForMember(x => x.ListService, opt => opt.MapFrom(src => src.ListService))
            .ForMember(x => x.Type, opt => opt.MapFrom(src => Enum.GetName(src.Type)))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<AdditionalServiceRequest, AbTrip.Request.AdditionalServiceRequest>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        #endregion
    }

    #region Private work

    private static string? ConvertToDatetimeRaw(DateTime? dateTime) => dateTime?.ToString("ddMMyyyy");

    #endregion
}