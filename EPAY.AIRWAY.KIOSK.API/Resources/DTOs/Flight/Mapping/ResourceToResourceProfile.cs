using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;
using AbTrip = EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip;

namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Mapping;

public sealed class ResourceToResourceProfile : Profile
{
    public ResourceToResourceProfile()
    {
        CreateMap<SearchRequest, AbTrip.Request.SearchFlightRequest>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));

        CreateMap<SearchFlightRequest, AbTrip.Request.SearchFlightInner>()
            .ForMember(x => x.DepartDate, opt => opt.MapFrom(src => ConvertToDatetimeRaw(src.DepartDate)))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));
        
        CreateMap<AbTrip.Response.SearchFlightResponse, SearchResponse>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));
        
        CreateMap<AbTrip.Response.SearchFlightInner, FareDataResponse>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));
        
        CreateMap<AbTrip.Response.FlightResponse, FlightResponse>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));
        
        CreateMap<AbTrip.Response.SegmentResponse, SegmentResponse>()
            .ForMember(x => x.Plane, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));
        
        CreateMap<AbTrip.Response.FareRuleResponse, FareRulesResponse>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));
        
        CreateMap<AbTrip.Response.RulesGroupResponse, RulesGroupResponse>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null && !string.IsNullOrEmpty(srcMember?.ToString())));
    }

    #region Private work

    private static string? ConvertToDatetimeRaw(DateTime? dateTime) => dateTime?.ToString("ddMMyyyy");

    #endregion
}