namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;

public sealed class BookingResponse
{
    public string? BillId { get; set; }
    public bool? IsPaylater { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int? TotalPrice { get; set; }
    public InvoiceResponse? Invoice { get; set; }
    public ContactResponse? Contact { get; set; }
    public List<PassengerResponse>? ListPassenger { get; set; }
    public List<BookingInnerResponse>? ListFareData { get; set; }
}

public sealed class BookingInnerResponse
{
    public AirportResponse? StartPoint { get; set; }
    public AirportResponse? EndPoint { get; set; }
    
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    public int? FareDataId { get; set; }
    
    public int? Adt { get; set; }
    public int? Chd { get; set; }
    public int? Inf { get; set; }
    
    public int? UnitPriceAdt { get; set; }
    public int? UnitPriceChd { get; set; }
    public int? UnitPriceInf { get; set; }
    
    public int? TotalPrice { get; set; }
    
    public string? FlightNumber { get; set; }

    public AirlineResponse? Airline { get; set; }
}

public sealed class PassengerResponse
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
    /// Giới tính: <br/>
    /// true – nam <br/>
    /// false – nữ <br/>
    /// </summary>
    public bool? Gender { get; set; }
    
    /// <summary>
    /// Phân loại hành khách
    /// </summary>
    public MyEnum.PassengerType Type { get; set; }

    /// <summary>
    /// Danh sách hành lý ký gửi
    /// </summary>
    public List<BaggageResponse>? ListBaggage { get; set; }

    /// <summary>
    /// Danh sách dịch vụ bổ sung
    /// </summary>
    public List<AncillaryResponse>? ListService { get; set; }
}

public sealed class ContactResponse
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

public sealed class InvoiceResponse
{
    /// <summary>
    /// Mã số thuế
    /// </summary>
    public string? TaxCode { get; set; }

    /// <summary>
    /// Tên doanh nghiệp
    /// </summary>
    public string? CompanyNameReceive { get; set; }

    /// <summary>
    /// Địa chỉ người nhận
    /// </summary>
    public string? AddressReceive { get; set; }

    /// <summary>
    /// Tên thành phố (người nhận)
    /// </summary>
    public string? CityNameReceive { get; set; }

    /// <summary>
    /// Tên người đại diện
    /// </summary>
    public string? ReceiverReceive { get; set; }
}