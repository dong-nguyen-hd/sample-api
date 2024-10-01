using System.Globalization;

namespace EPAY.AIRWAY.KIOSK.API.Extensions;

using TimeZoneConverter;

public static class RelateDateTime
{
    public static DateTime? ConvertToDatetimeWithOffset(this DateTime? utc, string? rawOffset)
    {
        // Validate
        if (utc == null || string.IsNullOrEmpty(rawOffset))
            return null;

        // Extract hours and minutes from the offset string
        TimeSpan offset = ParseTimeZoneOffset(rawOffset);

        // Convert UTC time to local time using the dynamic offset
        return utc + offset;

        static TimeSpan ParseTimeZoneOffset(string offsetString)
        {
            // Remove the '+' or '-' sign and split by ':'
            var sign = offsetString[0] == '+' ? 1 : -1;
            var parts = offsetString.Substring(1).Split(':');
        
            // Parse hours and minutes
            int hours = int.Parse(parts[0]) * sign;
            int minutes = int.Parse(parts[1]);
        
            return new TimeSpan(hours, minutes, 0);
        }
    }

    /// <summary>
    /// Chức năng: chuyển đổi DateTime về string theo định dạng hệ thống quy định
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string ConvertToSystemFormat(this DateTime data)
        => data.ToString(SystemConstant.SystemFormatDatetime);

    /// <summary>
    /// Chức năng: chuyển đổi kiểu dữ liệu string về Datetime theo format quy định
    /// </summary>
    /// <param name="source"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static DateTime? ConvertStringToDatetime(this string? source, string? format) =>
        DateTime.TryParseExact(source ?? throw new ArgumentException(nameof(source)), format ?? throw new ArgumentException(nameof(format)),
            CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDatetime)
            ? parsedDatetime
            : default;

    /// <summary>
    /// Chức năng: chuyển đổi giờ UTC về giờ +7:00
    /// </summary>
    /// <param name="utc"></param>
    /// <returns></returns>
    public static DateTime ConvertUtcToVietnamTz(this DateTime utc)
    {
        TimeZoneInfo localTz = TZConvert.GetTimeZoneInfo(SystemConstant.VietnamTimeZoneId);
        return TimeZoneInfo.ConvertTimeFromUtc(utc, localTz);
    }

    /// <summary>
    /// Chức năng: chuyển đổi giờ +7:00 về UTC
    /// </summary>
    /// <param name="localTime"></param>
    /// <returns></returns>
    public static DateTime ConvertVietnamTzToUtc(this DateTime localTime)
    {
        TimeZoneInfo localTz = TZConvert.GetTimeZoneInfo(SystemConstant.VietnamTimeZoneId);
        return TimeZoneInfo.ConvertTimeToUtc(localTime, localTz);
    }
}