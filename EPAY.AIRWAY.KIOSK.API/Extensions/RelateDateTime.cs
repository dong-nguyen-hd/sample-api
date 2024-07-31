namespace EPAY.AIRWAY.KIOSK.API.Extensions;

using TimeZoneConverter;

public static class RelateDateTime
{
    /// <summary>
    /// Chức năng: chuyển đổi DateTime về string theo định dạng hệ thống quy định
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string ConvertToSystemFormat(this DateTime data)
        => data.ToString(SystemConstant.SystemFormatDatetime);

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