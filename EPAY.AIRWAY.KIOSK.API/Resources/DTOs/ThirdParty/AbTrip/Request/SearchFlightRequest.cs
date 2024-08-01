namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.AbTrip.Request;

public sealed class SearchFlightRequest : BaseRequest
{
    /// <summary>
    /// Số lượng khách người lớn
    /// </summary>
    [JsonPropertyName("Adt")]
    public int? Adt { get; set; }

    /// <summary>
    /// Số lượng khách trẻ em
    /// </summary>
    [JsonPropertyName("Chd")]
    public int? Chd { get; set; }

    /// <summary>
    /// Số lượng khách trẻ sơ sinh
    /// </summary>
    [JsonPropertyName("Inf")]
    public int? Inf { get; set; }

    /// <summary>
    /// Chế độ hiển thị giá (mặc định để trống)
    /// </summary>
    [JsonPropertyName("ViewMode")]
    public string? ViewMode { get; set; }

    /// <summary>
    /// Danh sách thông tin các chặng bay muốn tìm kiếm
    /// </summary>
    [JsonPropertyName("ListFlight")]
    public List<SearchFlightInner>? ListFlight { get; set; }
}

public sealed class SearchFlightInner
{
    /// <summary>
    /// Mã hãng hàng không. Để trống nếu muốn tìm tất cả.
    /// </summary>
    [JsonPropertyName("Airline")]
    public string? Airline { get; set; }

    /// <summary>
    /// Mã sân bay, thành phố đi
    /// </summary>
    [JsonPropertyName("StartPoint")]
    public string? StartPoint { get; set; }

    /// <summary>
    /// Mã sân bay, thành phố đến
    /// </summary>
    [JsonPropertyName("EndPoint")]
    public string? EndPoint { get; set; }

    /// <summary>
    /// Ngày khởi hành (định dạng ddMMyyyy)
    /// </summary>
    [JsonPropertyName("DepartDate")]
    public string? DepartDate { get; set; }
}