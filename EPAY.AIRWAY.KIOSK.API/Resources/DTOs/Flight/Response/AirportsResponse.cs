namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;

public sealed class AirportsResponse
{
    public int? Id { get; set; }
    
    public string? Code { get; set; }

    public string? NameVi { get; set; }

    public string? NameEn { get; set; }

    public string? CityCode { get; set; }

    public string? CityNameVi { get; set; }

    public string? CityNameEn { get; set; }

    public string? CountryCode { get; set; }

    public string? CountryNameVi { get; set; }

    public string? CountryNameEn { get; set; }

    public string? ContinentCode { get; set; }

    public string? ContinentNameVi { get; set; }

    public string? ContinentNameEn { get; set; }
}