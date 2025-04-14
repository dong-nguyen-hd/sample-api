using AIRWAY.KIOSK.API.Extensions.JsonConverter;
using AIRWAY.KIOSK.API.Resources.Exceptions;

namespace AIRWAY.KIOSK.API.Resources.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class CustomDateTimeAttribute(string format) : JsonConverterAttribute
{
    public override JsonConverter CreateConverter(Type typeToConvert)
    {
        if (typeToConvert == typeof(DateTime) || typeToConvert == typeof(DateTime?))
            return new CustomTimestampConverter(format);
        
        if (typeToConvert == typeof(DateTimeOffset) || typeToConvert == typeof(DateTimeOffset?))
            return new CustomTimestampConverter(format);

        throw new MessageResultException($"Converter not work with {typeToConvert.Name}.");
    }
}