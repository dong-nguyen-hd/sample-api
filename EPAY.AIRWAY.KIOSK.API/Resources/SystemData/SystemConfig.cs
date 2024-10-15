namespace EPAY.AIRWAY.KIOSK.API.Resources.SystemData;

public static class SystemConfig
{
    #region System

    public const string SystemExpiredLogDays = "SYSTEM_EXPIRED_LOG_DAYS";
    public const string SystemExpiredTokenDays = "SYSTEM_EXPIRED_TOKEN_DAYS";
    public const string SystemBeHost = "SYSTEM_BE_HOST";
    public const string SystemFeHost = "SYSTEM_FE_HOST";

    public const string SystemEmailAddressTo = "SYSTEM_EMAIL_ADDRESS_TO";
    public const string SystemEmailAddressFrom = "SYSTEM_EMAIL_ADDRESS_FROM";
    public const string SystemEmailAddressCC = "SYSTEM_EMAIL_ADDRESS_CC";
    public const string SystemEmailAddressBCC = "SYSTEM_EMAIL_ADDRESS_BCC";
    public const string SystemEmailPassword = "SYSTEM_EMAIL_PASSWORD";
    public const string SystemEmailHost = "SYSTEM_EMAIL_HOST";

    #endregion

    #region Payment Gateway

    public const string PaymentGatewayLogin = "PAYMENT_GATEWAY_LOGIN";
    public const string PaymentGatewayCheckStatus = "PAYMENT_GATEWAY_CHECK_STATUS";
    public const string PaymentGatewayCreateOrder = "PAYMENT_GATEWAY_CREATE_ORDER";
    public const string PaymentGatewayRefund = "PAYMENT_GATEWAY_REFUND";
    public const string PaymentGatewayBaseAddress = "PAYMENT_GATEWAY_BASE_ADDRESS";
    public const string PaymentGatewayEnableVerifyTls = "PAYMENT_GATEWAY_ENABLE_VERIFY_TLS";
    public const string PaymentGatewayAgencyCode = "PAYMENT_GATEWAY_AGENCY_CODE";
    public const string PaymentGatewayOrderDescription = "PAYMENT_GATEWAY_ORDER_DESCRIPTION";
    public const string PaymentGatewayTimeLimitBankAccount = "PAYMENT_GATEWAY_TIME_LIMIT_BANK_ACCOUNT";
    public const string PaymentGatewayTimeLimitCard = "PAYMENT_GATEWAY_TIME_LIMIT_CARD";
    public const string PaymentGatewayTimeLimitQr = "PAYMENT_GATEWAY_TIME_LIMIT_QR";
    public const string PaymentGatewayTimeLimitPos = "PAYMENT_GATEWAY_TIME_LIMIT_POS";
    public const string PaymentGatewayTimeLimitEpayWallet = "PAYMENT_GATEWAY_TIME_LIMIT_EPAY_WALLET";
    public const string PaymentGatewayClientIp = "PAYMENT_GATEWAY_CLIENT_IP";
    public const string PaymentGatewaySecretKey = "PAYMENT_GATEWAY_SECRET_KEY";
    public const string PaymentGatewayPublicKey = "PAYMENT_GATEWAY_PUBLIC_KEY";
    public const string PaymentGatewayPrivateKeyForBe = "PAYMENT_GATEWAY_PRIVATE_KEY_FOR_BE";
    public const string PaymentGatewayPublicKeyForBe = "PAYMENT_GATEWAY_PUBLIC_KEY_FOR_BE";
    public const string PaymentGatewayPassword = "PAYMENT_GATEWAY_PASSWORD";
    public const string PaymentGatewayAccount = "PAYMENT_GATEWAY_ACCOUNT";
    public const string PaymentGatewayMerchantCode = "PAYMENT_GATEWAY_MERCHANT_CODE";

    #endregion

    #region AbTrip

    public const string AbTripEnableVerifyTls = "ABTRIP_ENABLE_VERIFY_TLS";
    public const string AbTripBaseAddress = "ABTRIP_BASE_ADDRESS";
    public const string AbTripSearchFlight = "ABTRIP_SEARCH_FLIGHT";
    public const string AbTripBaggage = "ABTRIP_BAGGAGE";
    public const string AbTripAncillary = "ABTRIP_ANCILLARY";
    public const string AbTripFareRules = "ABTRIP_FARE_RULES";
    public const string AbTripVerifyFlight = "ABTRIP_VERIFY_FLIGHT";
    public const string AbTripPriceQuote = "ABTRIP_PRICE_QUOTE";
    public const string AbTripBookFlight = "ABTRIP_BOOK_FLIGHT";
    public const string AbTripAircrafts = "ABTRIP_AIRCRAFTS";
    public const string AbTripAirports = "ABTRIP_AIRPORTS";
    public const string AbTripAirlines = "ABTRIP_AIRLINES";
    public const string AbTripIssue = "ABTRIP_ISSUE";
    public const string AbTripOrderInfo = "ABTRIP_ORDER_INFO";
    public const string AbTripUsername = "ABTRIP_USERNAME";
    public const string AbTripPassword = "ABTRIP_PASSWORD";

    #endregion
}