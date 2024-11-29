using System.Globalization;

namespace EPAY.AIRWAY.KIOSK.API.Extensions.JsonConverter;

public sealed class CustomTimestampConverter(string format) : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var isValid = DateTimeOffset.TryParseExact(reader.GetString(), format, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTimeOffset date);
        return isValid ? date : DateTimeOffset.MinValue;
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}