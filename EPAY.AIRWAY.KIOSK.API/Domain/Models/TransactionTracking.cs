using EPAY.AIRWAY.KIOSK.API.Domain.Models.Base;
using EPAY.AIRWAY.KIOSK.API.Resources.Enums;
using IdGen;

namespace EPAY.AIRWAY.KIOSK.API.Domain.Models;

/// <summary>
/// Thông tin cập nhật giao dịch
/// </summary>
public sealed class TransactionTracking : BaseModel
{
    public string Id { get; set; } = RelateText.GenId();

    public string? TraceId
    {
        get => _traceId;
        set => _traceId = value?.Replace(':', '_');
    }

    private string? _traceId;
    
    public ServiceStatus ServiceProviderStatus { get; set; }
    
    public PaymentStatus PaymentProviderStatus { get; set; }
    
    public string? Message { get; set; }

    public PaymentTransaction PaymentTransaction { get; set; }
    public string PaymentTransactionId { get; set; }
}