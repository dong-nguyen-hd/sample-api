namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip.Request;

public sealed class BookFlightRequest : BaseRequest
{
    [JsonPropertyName("AutoIssue")]
    public bool? AutoIssue { get; set; }
    
    [JsonPropertyName("BookType")]
    public string? BookType { get; set; }

    [JsonPropertyName("UseAgentContact")]
    public string? UseAgentContact { get; set; }

    [JsonPropertyName("Contact")]
    public ContactRequest? Contact { get; set; }

    [JsonPropertyName("ListPassenger")]
    public List<PassengerRequest> ListPassenger { get; set; }
}

public sealed class ContactRequest
{
    [JsonPropertyName("FirstName")]
    public string? FirstName { get; set; }

    [JsonPropertyName("LastName")]
    public string? LastName { get; set; }

    [JsonPropertyName("Gender")]
    public string? Gender { get; set; }

    [JsonPropertyName("Phone")]
    public string? Phone { get; set; }

    [JsonPropertyName("Email")]
    public string? Email { get; set; }
}

public sealed class PassengerRequest
{
    [JsonPropertyName("Index")]
    public int? Index { get; set; }

    [JsonPropertyName("FirstName")]
    public string? FirstName { get; set; }

    [JsonPropertyName("LastName")]
    public string? LastName { get; set; }

    [JsonPropertyName("Type")]
    public string? Type { get; set; }

    [JsonPropertyName("IdCard")]
    public string? IdCard { get; set; }

    [JsonPropertyName("PassportNumber")]
    public string? PassportNumber { get; set; }

    [JsonPropertyName("Gender")]
    public bool? Gender { get; set; }

    [JsonPropertyName("Membership")]
    public string? Membership { get; set; }

    [JsonPropertyName("Birthday")]
    public string? Birthday { get; set; }
}