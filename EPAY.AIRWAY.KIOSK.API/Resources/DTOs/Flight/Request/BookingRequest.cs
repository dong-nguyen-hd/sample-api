namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Request;

public sealed class BookingRequest
{
    /// <summary>
    /// Xác định giao dịch sử dụng thanh toán sau: <br/>
    /// true - có sử dụng <br/>
    /// false - không sử dụng <br/>
    /// </summary>
    public bool? IsPaylater { get; set; }
    
    /// <summary>
    /// Múi giờ tại địa điểm khởi hành
    /// </summary>
    public string? StartTimeZoneOffset { get; set; }
    
    /// <summary>
    /// Thông tin liên hệ
    /// </summary>
    public ContactRequest? Contact { get; set; }
    
    /// <summary>
    /// Thông tin hoá đơn điện tử
    /// </summary>
    public InvoiceRequest? Invoice { get; set; }
    
    /// <summary>
    /// Danh sách hành khách
    /// </summary>
    public List<PassengerRequest>? ListPassenger { get; set; }

    public List<FareDataRequest>? ListFareData { get; set; }
}

public sealed class InvoiceRequest
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

public sealed class ContactRequest
{
    /// <summary>
    /// Tên người liên hệ
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Họ người liên hệ
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
    /// Tên
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Họ
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
    public string? Session { get; set; }

    public string? Type { get; set; }

    public string? Description { get; set; }

    public string? StartPoint { get; set; }

    public string? EndPoint { get; set; }

    public string? StatusCode { get; set; }

    public bool? Confirmed { get; set; }

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