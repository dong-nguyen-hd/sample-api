namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip.Response;

public sealed class AirlinesResponse : BaseResponse
{
    public List<AirlinesInnerResponse>? Data { get; set; }
}

public sealed class AirlinesInnerResponse
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("name_en")]
    public string? NameEn { get; set; }

    [JsonPropertyName("logo")]
    public string? Logo { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("group_name")]
    public string? GroupName { get; set; }

    [JsonPropertyName("group_name_en")]
    public string? GroupNameEn { get; set; }

    [JsonPropertyName("id")]
    public int? Id { get; set; }
}