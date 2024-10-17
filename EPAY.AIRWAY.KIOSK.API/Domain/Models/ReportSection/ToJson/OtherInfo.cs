namespace EPAY.AIRWAY.KIOSK.API.Domain.Models.ReportSection.ToJson;

public sealed class OtherInfo
{
    [JsonPropertyName("orderId")]
    public string? OrderId { get; set; }

    [JsonPropertyName("startPoint")]
    public string? StartPoint { get; set; }

    [JsonPropertyName("endPoint")]
    public string? EndPoint { get; set; }

    [JsonPropertyName("journeyType")]
    public string? JourneyType { get; set; }

    [JsonPropertyName("ticketType")]
    public string? TicketType { get; set; }

    [JsonPropertyName("listFareData")]
    public List<FareData>? ListFareData { get; set; }
}

public sealed class FareData
{
    [JsonPropertyName("isDeparture")]
    public bool? IsDeparture { get; set; }
    
    [JsonPropertyName("bookingCode")]
    public string? BookingCode { get; set; }

    [JsonPropertyName("ticketQuantityAdt")]
    public string? TicketQuantityAdt { get; set; }

    [JsonPropertyName("ticketNumberAdt")]
    public string? TicketNumberAdt { get; set; }

    [JsonPropertyName("ticketQuantityChd")]
    public string? TicketQuantityChd { get; set; }

    [JsonPropertyName("ticketNumberChd")]
    public string? TicketNumberChd { get; set; }

    [JsonPropertyName("serviceProviderStatus")]
    public bool? ServiceProviderStatus { get; set; }

    [JsonPropertyName("totalPrice")]
    public long? TotalPrice { get; set; }

    [JsonPropertyName("listBaggage")]
    public List<ServiceData>? ListBaggage { get; set; }

    [JsonPropertyName("listAncillary")]
    public List<ServiceData>? ListAncillary { get; set; }
}

public sealed class ServiceData
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("price")]
    public long? Price { get; set; }
}