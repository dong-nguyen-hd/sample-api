namespace AIRWAY.KIOSK.API.Resources.DTOs.Administrative.Response;

public sealed class ProvinceResponse
{
    public static List<ProvinceInnerResponse> Provinces { get; private set; }

    public static int Temp { get; private set; }
}

public sealed class ProvinceInnerResponse
{
    public string Name { get; private set; }
    public string NameWithType { get; private set; }
    public string Code { get; private set; }
}