namespace AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.IACV.Application;

public record PaylaterUri
{
    public string? HostFe { get; set; }
    public string? PaylaterEndpoint { get; set; }
    public MyEnum.PlatformType? PlatformType { get; set; }
}