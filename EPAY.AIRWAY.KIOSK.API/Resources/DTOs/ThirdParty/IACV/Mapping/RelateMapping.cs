using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.Flight.Response;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.IACV.Application;
using EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.IACV.Response;
using EPAY.AIRWAY.KIOSK.API.Resources.Exceptions;

namespace EPAY.AIRWAY.KIOSK.API.Resources.DTOs.ThirdParty.IACV.Mapping;

public static class RelateMapping
{
    public static List<FlightInformationResponse?>? MappingModelToResouce(List<Model.Bill>? bills,
        MasterDataResponse? masterData,
        PaylaterUri? paylaterUri)
    {
        if (bills == null ||
            bills.Count <= 0 ||
            masterData == null ||
            paylaterUri == null)
            return default;

        List<FlightInformationResponse?> result = new();

        foreach (var bill in bills)
            result.Add(MappingModelToResouce(bill, masterData, paylaterUri));

        return result;
    }

    public static FlightInformationResponse? MappingModelToResouce(Model.Bill? bill,
        MasterDataResponse? masterData,
        PaylaterUri? paylaterUri)
    {
        if (bill == null || masterData == null || paylaterUri == null)
            return default;

        FlightInformationResponse result = new()
        {
            OrderId = bill.AbTripOrderId,
            OrderDatetimeUtc = bill.CreatedDatetimeUtc,
            TicketType = ConvertTicketType(bill.FlightType),
        };

        // Gán thông tin chuyến bay chiều đi
        var flightStart = bill.FlightDatas?.FirstOrDefault(x => x.Departure);
        if (flightStart != null)
        {
            result.FlightStart = new()
            {
                BookingCode = flightStart.BookingCode,
                StartDate = flightStart.StartDate,
                EndDate = flightStart.EndDate,
                FlightNumber = flightStart.FlightNumber,
                Airline = masterData?.Airlines?.Find(x => x.Code.Equals(flightStart.Airline)),
                StartPoint = masterData?.Airports?.Find(x => x.Code.Equals(flightStart.StartPoint)),
                EndPoint = masterData?.Airports?.Find(x => x.Code.Equals(flightStart.EndPoint))
            };
        }

        // Gán thông tin chuyến bay chiều về
        var flightEnd = bill.FlightDatas?.FirstOrDefault(x => !x.Departure);
        if (flightEnd != null)
        {
            result.FlightEnd = new()
            {
                BookingCode = flightEnd.BookingCode,
                StartDate = flightEnd.StartDate,
                EndDate = flightEnd.EndDate,
                FlightNumber = flightEnd.FlightNumber,
                Airline = masterData?.Airlines?.Find(x => x.Code.Equals(flightEnd.Airline)),
                StartPoint = masterData?.Airports?.Find(x => x.Code.Equals(flightEnd.StartPoint)),
                EndPoint = masterData?.Airports?.Find(x => x.Code.Equals(flightEnd.EndPoint))
            };
        }

        // Gán thông tin thanh toán
        var paymentTransaction = bill?.PaymentTransactions?.FirstOrDefault(x => x.PaymentProviderStatus == MyEnum.PaymentStatus.Success);
        result.PaymentInformation = new()
        {
            PaymentType = paymentTransaction?.PaymentType ?? MyEnum.PaymentType.PayLater,
            OrderCode = paymentTransaction?.OrderCode,
            PaidDatetimeUtc = paymentTransaction?.PaidDatetimeUtc,
            TotalAmount = paymentTransaction?.TotalAmount ?? 0,
            ExpiredDatetimeUtc = bill?.ExpiredDatetimeUtc,
            PaylaterUri = ComputePaylaterUri(paylaterUri, bill?.AbTripOrderId)
        };

        return result;
    }

    #region Private work

    private static string? ComputePaylaterUri(PaylaterUri? paylaterUri, string? abtripOrderId)
    {
        if (paylaterUri == null ||
            string.IsNullOrEmpty(paylaterUri.HostFe) ||
            string.IsNullOrEmpty(paylaterUri.PaylaterEndpoint) ||
            paylaterUri.PlatformType == null ||
            string.IsNullOrEmpty(abtripOrderId))
            return null;

        return $"{paylaterUri.HostFe}{string.Format(paylaterUri.PaylaterEndpoint, (int)paylaterUri.PlatformType, abtripOrderId)}";
    }

    private static MyEnum.TicketType ConvertTicketType(MyEnum.FlightType source)
    {
        switch (source)
        {
            case MyEnum.FlightType.DomesticOneWay:
            case MyEnum.FlightType.InternationalOneWay:
                return MyEnum.TicketType.Oneway;
            case MyEnum.FlightType.DomesticRoundTrip:
            case MyEnum.FlightType.InternationalRoundTrip:
                return MyEnum.TicketType.Roundtrip;
            default:
                throw new MessageResultException("Loại chuyến bay không hợp lệ");
        }
    }

    #endregion
}