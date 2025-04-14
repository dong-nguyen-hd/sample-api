namespace AIRWAY.KIOSK.API.Resources.DTOs.Payment.Request;

public sealed class CustomerRequest
{
    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? Mobile { get; set; }

    public string? Address { get; set; }

    public string? IdNumber { get; set; }
}