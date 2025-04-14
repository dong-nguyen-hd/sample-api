namespace AIRWAY.KIOSK.API.Resources.DTOs.Report.Response;

public sealed class TemplatePaymentResponse
{
    public List<TemplatePaymentInner>? Report { get; set; }
}

public sealed class TemplatePaymentInner
{
    [JsonPropertyOrder(1)]
    [JsonPropertyName("STT")]
    public int Record { get; set; }
    
    [JsonPropertyOrder(2)]
    [JsonPropertyName("Kênh dịch vụ")]
    public string? SaleChannel { get; set; }
    
    [JsonPropertyOrder(3)]
    [JsonPropertyName("Đối tác dịch vụ")]
    public string? ServicePartner { get; set; }
    
    [JsonPropertyOrder(4)]
    [JsonPropertyName("Mã điểm bán")]
    public string? Location { get; set; }
    
    [JsonPropertyOrder(5)]
    [JsonPropertyName("Mã thiết bị")]
    public string? DeviceSerial { get; set; }
    
    /// <summary>
    /// Định dạng: dd/MM/yyyy HH:mm:ss
    /// </summary>
    [JsonPropertyOrder(6)]
    [JsonPropertyName("Ngày giao dịch")]
    public string? CreatePaymentDatetime { get; set; }
    
    [JsonPropertyOrder(7)]
    [JsonPropertyName("Mã giao dịch")]
    public string? OrderCode { get; set; }
    
    [JsonPropertyOrder(8)]
    [JsonPropertyName("Mã đơn hàng")]
    public string? AbTripOrderId { get; set; }
    
    [JsonPropertyOrder(9)]
    [JsonPropertyName("Hành trình")]
    public string? Route { get; set; }
    
    [JsonPropertyOrder(10)]
    [JsonPropertyName("Loại hành trình")]
    public string? RouteType { get; set; }
    
    [JsonPropertyOrder(11)]
    [JsonPropertyName("Loại vé")]
    public string? TicketType { get; set; }
    
    [JsonPropertyOrder(12)]
    [JsonPropertyName("Số lượng vé (Người lớn)")]
    public string? TicketQuantityAdt { get; set; }
    
    [JsonPropertyOrder(13)]
    [JsonPropertyName("Mã vé (Người lớn)")]
    public string? TicketNumberAdt { get; set; }
    
    [JsonPropertyOrder(14)]
    [JsonPropertyName("Số lượng vé (Trẻ em)")]
    public string? TicketQuantityChd { get; set; }
    
    [JsonPropertyOrder(15)]
    [JsonPropertyName("Mã vé (Trẻ em)")]
    public string? TicketNumberChd { get; set; }
    
    [JsonPropertyOrder(16)]
    [JsonPropertyName("Giá hành trình")]
    public long? TotalPrice { get; set; }
    
    [JsonPropertyOrder(17)]
    [JsonPropertyName("Phương thức thanh toán")]
    public string? PaymentType { get; set; }
    
    [JsonPropertyOrder(18)]
    [JsonPropertyName("Trạng thái thanh toán")]
    public string? PaymentProviderStatus { get; set; }
    
    [JsonPropertyOrder(19)]
    [JsonPropertyName("Trạng thái xuất vé")]
    public string? ServiceProviderStatus { get; set; }
    
    [JsonPropertyOrder(20)]
    [JsonPropertyName("Ghi chú")]
    public string? Note { get; set; }
}