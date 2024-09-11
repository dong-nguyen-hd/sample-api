namespace EPAY.AIRWAY.KIOSK.API.Domain.Context.Config;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Chức năng: cấu hình schema cho bảng Configuration
/// </summary>
public sealed class ConfigurationConfig : IEntityTypeConfiguration<Models.Configuration>
{
    public void Configure(EntityTypeBuilder<Models.Configuration> entity)
    {
        entity.ToTable("tbl_configuration");
        entity.Property(x => x.CreatedDatetimeUtc).HasColumnType("timestamp without time zone");
        entity.Property(x => x.UpdatedDatetimeUtc).HasColumnType("timestamp without time zone");

        entity.HasKey(x => x.Id);
        entity.HasQueryFilter(x => x.Active);

        // Indexing
        entity.HasIndex(x => x.Key).IsUnique();
        entity.HasIndex(x => new { x.Internal, x.Active, x.Key });
        entity.HasIndex(x => new { x.Key, x.Active, x.Internal });

        entity.HasData(new[]
        {
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.PaymentGatewayMerchantCode,
                Value = "<MERCHANT_CODE>",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.PaymentGatewayAccount,
                Value = "<ACCOUNT>",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.PaymentGatewayPassword,
                Value = "<PASSWORD>",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.PaymentGatewayPublicKeyForBe,
                Value = "<PUBLIC_KEY_FOR_BE>",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.PaymentGatewayPrivateKeyForBe,
                Value = "<PRIVATE_KEY_FOR_BE>",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.PaymentGatewayPublicKey,
                Value = "<PUBLIC_KEY>",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.PaymentGatewaySecretKey,
                Value = "<SECRET_KEY>",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.PaymentGatewayClientIp,
                Value = "<CLIENT_IP>",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.PaymentGatewayTimeLimitQr,
                Value = "2",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.PaymentGatewayTimeLimitPos,
                Value = "1",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.PaymentGatewayTimeLimitEpayWallet,
                Value = "10",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.PaymentGatewayTimeLimitCard,
                Value = "10",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.PaymentGatewayTimeLimitBankAccount,
                Value = "10",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.PaymentGatewayOrderDescription,
                Value = "<ORDER_DESCRIPTION>",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.PaymentGatewayAgencyCode,
                Value = "<AGENCY_CODE>",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.PaymentGatewayEnableVerifyTls,
                Value = "true",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.PaymentGatewayBaseAddress,
                Value = "<BASE_ADDRESS>",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.PaymentGatewayRefund,
                Value = "gw/api/paymentgateway/merchant/refund",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.PaymentGatewayCreateOrder,
                Value = "gw/api/paymentgateway/merchant/create_order",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.PaymentGatewayCheckStatus,
                Value = "gw/api/paymentgateway/merchant/check_status",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.PaymentGatewayLogin,
                Value = "gw/api/paymentgateway/merchant/token",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.SystemFeHost,
                Value = "<FE_HOST>",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.SystemBeHost,
                Value = "<BE_HOST>",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.SystemExpiredTokenDays,
                Value = "7",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.SystemExpiredLogDays,
                Value = "45",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.AbTripEnableVerifyTls,
                Value = "true",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.AbTripBaseAddress,
                Value = "<AbTripBaseAddress>",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.AbTripSearchFlight,
                Value = "api/v2/flights/searchflight",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.AbTripBaggage,
                Value = "api/v2/flights/getbaggage",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.AbTripAncillary,
                Value = "api/v2/flights/getancillary",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.AbTripFareRules,
                Value = "api/v2/flights/getfarerules",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.AbTripVerifyFlight,
                Value = "api/v2/flights/verifyflight",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.AbTripPriceQuote,
                Value = "api/v2/flights/pricequote",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.AbTripBookFlight,
                Value = "api/v2/flights/bookflight",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.AbTripAircrafts,
                Value = "api/v2/aircrafts",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.AbTripAirports,
                Value = "api/v2/airports",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.AbTripAirlines,
                Value = "api/v2/airlines",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.AbTripIssue,
                Value = "api/v2/flights/issue",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.AbTripOrderInfo,
                Value = "api/v2/order/info",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.AbTripUsername,
                Value = "<AbTripUsername>",
            },
            new Models.Configuration
            {
                Active = true,
                CreatedDatetimeUtc = DateTime.UtcNow,
                UpdatedDatetimeUtc = DateTime.UtcNow,
                Internal = true,
                Key = SystemConfig.AbTripPassword,
                Value = "<AbTripPassword>",
            },
        });
    }
}