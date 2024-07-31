using System.Globalization;

namespace EPAY.AIRWAY.KIOSK.API.Extensions.JsonConverter;

public sealed class CustomDateTimeAbTripConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var isValid = DateTime.TryParseExact(reader.GetString(), "ddMMyyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime date);
        return isValid ? date : DateTime.MinValue;
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}