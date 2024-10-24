using EPAY.AIRWAY.KIOSK.API.Domain.Services;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.HealthCheck.Response;
using EPAY.AIRWAY.KIOSK.API.Services.CronJob;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EPAY.AIRWAY.KIOSK.API.Services;

public sealed class HealthCheckService(PaymentReportJob paymentReportJob,
    IAbTripService abTripService,
    IPaymentGatewayService paymentGatewayService) : BaseService, IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        DateTime utcNow = DateTime.UtcNow;

        // Kiểm tra tính sẵn sàng hệ thông
        var checkPaymentGateway = await CheckPaymentGatewayAsync(utcNow, cancellationToken);
        var checkAbTrip = await CheckAbTripAsync(utcNow, cancellationToken);
        var checkSendEmail = await CheckSendEmailAsync(utcNow, cancellationToken);

        // Thông báo các mã lỗi liên quan
        string messages = string.Empty;
        if (!checkPaymentGateway.Item1)
            messages = $"{nameof(ThirdPartyCheck.PaymentGateway)}: {checkPaymentGateway.Item2}";
        if (!checkAbTrip.Item1)
            messages = $"{messages} >>> {nameof(ThirdPartyCheck.AbTrip)}: {checkAbTrip.Item2}";
        if (!checkSendEmail.Item1)
            messages = $"{messages} >>> {nameof(ThirdPartyCheck.SendEmail)}: {checkSendEmail.Item2}";

        // Tạo kết quả health-check
        var result = GetBaseResult(CodeMessage._0000, new ThirdPartyCheck()
        {
            Database = true,
            PaymentGateway = checkPaymentGateway.Item1,
            AbTrip = checkAbTrip.Item1,
            SendEmail = checkSendEmail.Item1
        }, messages);

        return HealthCheckResult.Healthy(result.MySerialize());
    }

    /// <summary>
    /// Chức năng: kiểm tra trạng thái sẵn sàng của dịch vụ PaymentGateway bằng cách thử api login
    /// </summary>
    /// <param name="utcNow"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<(bool, string?)> CheckPaymentGatewayAsync(DateTime utcNow, CancellationToken cancellationToken = default)
    {
        try
        {
            var configResult = await paymentGatewayService.GetConfigDataAsync(cancellationToken);
            var loginResult = await paymentGatewayService.GetTokenAsync(new()
            {
                UserName = configResult.Config.Account,
                Password = configResult.Config.Password,
            }, utcNow, configResult, cancellationToken);

            if (loginResult.CodeMessage == CodeMessage._0000)
                return (true, null);

            return (false, loginResult.Message);
        }
        catch (Exception ex)
        {
            return (false, $"{utcNow.ConvertToSystemFormat()} >>> {ex.Message}");
        }
    }

    /// <summary>
    /// Chức năng: kiểm tra trạng thái sẵn sàng của dịch vụ AbTrip bằng cách thử api get-aircraft
    /// </summary>
    /// <param name="utcNow"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<(bool, string?)> CheckAbTripAsync(DateTime utcNow, CancellationToken cancellationToken = default)
    {
        try
        {
            var abtripResult = await abTripService.GetAircraftsAsync(cancellationToken);

            if (abtripResult.CodeMessage == CodeMessage._0000)
                return (true, null);

            return (false, abtripResult.Message);
        }
        catch (Exception ex)
        {
            return (false, $"{utcNow.ConvertToSystemFormat()} >>> {ex.Message}");
        }
    }
    
    /// <summary>
    /// Chức năng: kiểm tra trạng thái sẵn sàng của kết nối email
    /// </summary>
    /// <param name="utcNow"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<(bool, string?)> CheckSendEmailAsync(DateTime utcNow, CancellationToken cancellationToken = default)
    {
        try
        {
            await paymentReportJob.TestConnectAsync(cancellationToken);

            return (true, null);
        }
        catch (Exception ex)
        {
            return (false, $"{utcNow.ConvertToSystemFormat()} >>> {ex.Message}");
        }
    }
}