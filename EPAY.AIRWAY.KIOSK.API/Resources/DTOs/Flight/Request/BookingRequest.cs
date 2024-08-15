namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request;

public sealed class BookingRequest
{
    /// <summary>
    /// Thông tin liên hệ
    /// </summary>
    public ContactRequest? Contact { get; set; }
    
    /// <summary>
    /// Thông tin hoá đơn điện tử
    /// </summary>
    public Invoice? Invoice { get; set; }

    public List<BookingFareRequest>? ListFareData { get; set; }
}

public sealed class BookingFareRequest
{
    public string? Session { get; set; }
    public int? FareDataId { get; set; }
    public List<BookingFlightRequest>? ListFlight { get; set; }
}

public sealed class BookingFlightRequest
{
    public string? FlightValue { get; set; }
    public string? StartPoint { get; set; }
    public string? EndPoint { get; set; }

    /// <summary>
    /// Danh sách hành khách
    /// </summary>
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
    public string? FirstName { get; set; }

    /// <summary>
    /// Tên người liên hệ
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Danh xưng: <br/>
    /// true - ông <br/>
    /// false - bà
    /// </summary>
    public bool Gender { get; set; }

    /// <summary>
    ///  Số điện thoại
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    public string? Email { get; set; }
}

public sealed class PassengerRequest
{
    /// <summary>
    /// Họ
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Tên đệm và tên
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Ngày sinh
    /// </summary>
    public DateTime? Birthday { get; set; }

    /// <summary>
    /// Loại khách: ADT, CHD, INF
    /// </summary>
    public MyEnum.PassengerType Type { get; set; }

    /// <summary>
    /// Giới tính: <br/>
    /// true – nam <br/>
    /// false – nữ <br/>
    /// </summary>
    public bool? Gender { get; set; }

    /// <summary>
    /// Danh sách hành lý ký gửi
    /// </summary>
    public List<AdditionalServiceRequest>? ListBaggage { get; set; }

    /// <summary>
    /// Danh sách dịch vụ bổ sung
    /// </summary>
    public List<AdditionalServiceRequest>? ListService { get; set; }
}

public sealed class AdditionalServiceRequest
{
    public string? StartPoint { get; set; }

    public string? EndPoint { get; set; }

    /// <summary>
    /// Mã hàng hàng không
    /// </summary>
    public string? Airline { get; set; }

    /// <summary>
    /// Thứ tự chuyến bay
    /// </summary>
    public int? Leg { get; set; }

    /// <summary>
    /// Giá trị chặng bay
    /// </summary>
    public string? Route { get; set; }

    /// <summary>
    /// Mã gói hành lý
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Mã tiền tệ
    /// </summary>
    public string? Currency { get; set; }

    /// <summary>
    /// Tên gói hành lý 
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Giá
    /// </summary>
    public int? Price { get; set; }

    /// <summary>
    /// Giá tiền gói hành lý
    /// </summary>
    public string? Value { get; set; }
}