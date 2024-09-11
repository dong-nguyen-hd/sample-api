namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip.Response;

public sealed class IssueResponse : BaseResponse
{
    public List<IssueInnerResponse>? Data { get; set; }
}

public sealed class IssueInnerResponse
{
    public string? BookingCode { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
}