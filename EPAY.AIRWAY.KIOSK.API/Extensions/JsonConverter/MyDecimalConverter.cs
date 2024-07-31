namespace EPAY.AIRWAY.KIOSK.API.Extensions.JsonConverter;

public sealed class MyDecimalConverter : JsonConverter<decimal>
{
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return decimal.TryParse(reader.GetString() ?? string.Empty, out decimal parsedDecimal) ? parsedDecimal : decimal.Zero;
    }

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
    {
        if (value % 1 == 0)
            writer.WriteNumberValue((int)value);
        else
            writer.WriteNumberValue(value);
    }
}