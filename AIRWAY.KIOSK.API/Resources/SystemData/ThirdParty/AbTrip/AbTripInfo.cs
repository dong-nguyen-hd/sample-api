namespace AIRWAY.KIOSK.API.Resources.SystemData.ThirdParty.AbTrip;

public sealed record AbTripInfo
{
    public AbTripApi? Api { get; set; }
    public AbTripConfig? Config { get; set; }
}