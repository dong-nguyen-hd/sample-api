namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip.Response;

public sealed class AirportsResponse : BaseResponse
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("name_vi")]
    public string? NameVi { get; set; }

    [JsonPropertyName("name_en")]
    public string? NameEn { get; set; }

    [JsonPropertyName("city_code")]
    public string? CityCode { get; set; }

    [JsonPropertyName("city_name_vi")]
    public string? CityNameVi { get; set; }

    [JsonPropertyName("city_name_en")]
    public string? CityNameEn { get; set; }

    [JsonPropertyName("country_code")]
    public string? CountryCode { get; set; }

    [JsonPropertyName("country_name_vi")]
    public string? CountryNameVi { get; set; }

    [JsonPropertyName("country_name_en")]
    public string? CountryNameEn { get; set; }

    [JsonPropertyName("continent_code")]
    public string? ContinentCode { get; set; }

    [JsonPropertyName("continent_name_vi")]
    public string? ContinentNameVi { get; set; }

    [JsonPropertyName("continent_name_en")]
    public string? ContinentNameEn { get; set; }

    [JsonPropertyName("id")]
    public int? Id { get; set; }
}