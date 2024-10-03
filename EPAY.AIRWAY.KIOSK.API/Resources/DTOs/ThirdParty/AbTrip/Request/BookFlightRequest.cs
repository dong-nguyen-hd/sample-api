namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip.Request;

public sealed class BookFlightRequest : BaseRequest
{
    /// <summary>
    /// Kiểu đặt chỗ <br/>
    /// Mặc định sử sụng giá trị "book-normal" => đặt chỗ tiêu chuẩn
    /// </summary>
    [JsonPropertyName("BookType")]
    public string? BookType { get; set; } = "book-normal";

    /// <summary>
    /// Sử dụng thông tin liên hệ của đại lý khi đặt chỗ <br/>
    /// Mặc đinh: false
    /// </summary>
    [JsonPropertyName("UseAgentContact")]
    public bool UseAgentContact { get; set; }

    /// <summary>
    /// Thông tin liên hệ
    /// </summary>
    [JsonPropertyName("Contact")]
    public ContactRequest? Contact { get; set; }

    /// <summary>
    /// Hoá đơn điện tử
    /// </summary>
    [JsonPropertyName("Invoice")]
    public Invoice? Invoice { get; set; }

    /// <summary>
    /// Danh sách hành khách
    /// </summary>
    [JsonPropertyName("ListPassenger")]
    public List<PassengerRequest>? ListPassenger { get; set; }
}

public sealed class Invoice
{
    /// <summary>
    /// Mã số thuế
    /// </summary>
    [JsonPropertyName("TaxCode")]
    public string? TaxCode { get; set; }

    /// <summary>
    /// Tên doanh nghiệp
    /// </summary>
    [JsonPropertyName("CompanyNameReceive")]
    public string? CompanyNameReceive { get; set; }

    /// <summary>
    /// Địa chỉ người nhận
    /// </summary>
    [JsonPropertyName("AddressReceive")]
    public string? AddressReceive { get; set; }

    /// <summary>
    /// Tên thành phố (người nhận)
    /// </summary>
    [JsonPropertyName("CityNameReceive")]
    public string? CityNameReceive { get; set; }

    /// <summary>
    /// Tên người đại diện
    /// </summary>
    [JsonPropertyName("ReceiverReceive")]
    public string? ReceiverReceive { get; set; }
}

public sealed class ContactRequest
{
    /// <summary>
    /// Họ người liên hệ
    /// </summary>
    [JsonPropertyName("FirstName")]
    public string? FirstName { get; set; }

    /// <summary>
    /// Tên người liên hệ
    /// </summary>
    [JsonPropertyName("LastName")]
    public string? LastName { get; set; }

    /// <summary>
    /// Danh xưng: <br/>
    /// true - ông <br/>
    /// false - bà
    /// </summary>
    [JsonPropertyName("Gender")]
    public bool Gender { get; set; }

    /// <summary>
    /// Số điện thoại
    /// </summary>
    [JsonPropertyName("Phone")]
    public string? Phone { get; set; }

    /// <summary>
    /// Mã vùng điện thoại
    /// </summary>
    [JsonPropertyName("Area")]
    public string? Area { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    [JsonPropertyName("Email")]
    public string? Email { get; set; }
}

public sealed class PassengerRequest
{
    /// <summary>
    /// Số thứ tự trong danh sách
    /// </summary>
    [JsonPropertyName("Index")]
    public int? Index { get; set; }

    /// <summary>
    /// Họ
    /// </summary>
    [JsonPropertyName("FirstName")]
    public string? FirstName { get; set; }

    /// <summary>
    /// Tên đệm và tên
    /// </summary>
    [JsonPropertyName("LastName")]
    public string? LastName { get; set; }

    /// <summary>
    /// Loại khách: ADT, CHD, INF
    /// </summary>
    [JsonPropertyName("Type")]
    public string? Type { get; set; }

    /// <summary>
    /// Giới tính: <br/>
    /// true – nam <br/>
    /// false – nữ <br/>
    /// </summary>
    [JsonPropertyName("Gender")]
    public bool? Gender { get; set; }

    /// <summary>
    /// Ngày sinh. Định dạng "ddMMyyyy"
    /// </summary>
    [JsonPropertyName("Birthday")]
    public string? Birthday { get; set; }

    /// <summary>
    /// Danh sách hành lý ký gửi
    /// </summary>
    [JsonPropertyName("ListBaggage")]
    public List<AdditionalServiceRequest>? ListBaggage { get; set; }

    /// <summary>
    /// Danh sách dịch vụ bổ sung
    /// </summary>
    [JsonPropertyName("ListService")]
    public List<AdditionalServiceRequest>? ListService { get; set; }
}

public sealed class AdditionalServiceRequest
{
    /// <summary>
    /// Mã hàng hàng không
    /// </summary>
    [JsonPropertyName("Airline")]
    public string? Airline { get; set; }

    /// <summary>
    /// Thứ tự chuyến bay
    /// </summary>
    [JsonPropertyName("Leg")]
    public int? Leg { get; set; }

    /// <summary>
    /// Giá trị chặng bay
    /// </summary>
    [JsonPropertyName("Route")]
    public string? Route { get; set; }

    /// <summary>
    /// Mã gói hành lý
    /// </summary>
    [JsonPropertyName("Code")]
    public string? Code { get; set; }

    /// <summary>
    /// Mã tiền tệ
    /// </summary>
    [JsonPropertyName("Currency")]
    public string? Currency { get; set; }

    /// <summary>
    /// Tên gói hành lý 
    /// </summary>
    [JsonPropertyName("Name")]
    public string? Name { get; set; }

    /// <summary>
    /// Giá tiền gói hành lý
    /// </summary>
    [JsonPropertyName("Price")]
    public int? Price { get; set; }

    /// <summary>
    /// Mã giá trị
    /// </summary>
    [JsonPropertyName("Value")]
    public string? Value { get; set; }
}