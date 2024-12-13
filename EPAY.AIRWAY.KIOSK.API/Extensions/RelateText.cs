using System.Globalization;
using System.Text;
using IdGen;

namespace EPAY.AIRWAY.KIOSK.API.Extensions;

using System.Text.RegularExpressions;

public static class RelateText
{
    #region Normalize

    public static bool ContainsVietnameseString(this string? target, string? source)
    {
        string? normalizedSource = NormalizeVietnameseString(source);
        string? normalizedTarget = NormalizeVietnameseString(target);

        if (string.IsNullOrEmpty(normalizedSource) || string.IsNullOrEmpty(normalizedTarget))
            return false;

        return normalizedSource.Contains(normalizedTarget);
    }

    private static string? NormalizeVietnameseString(this string? input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        string normalized = input.Normalize(NormalizationForm.FormD);

        StringBuilder builder = new StringBuilder();
        foreach (char c in normalized)
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                builder.Append(c);

        return builder.ToString().ToLowerInvariant();
    }

    #endregion

    #region GenId

    private static readonly IdGenerator _genId = new(Random.Shared.Next(0, 1023));

    /// <summary>
    /// Chức năng: tạo id
    /// </summary>
    /// <returns></returns>
    public static string GenId() => _genId.CreateId().ToString();

    #endregion

    /// <summary>
    /// Chức năng: xoá các kí tự khoảng trắng bị lặp lại (2 kí tự space -> 1 kí tự space)
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string RemoveSpaceCharacter(this string? text) =>
        string.IsNullOrEmpty(text) ? string.Empty : Regex.Replace(text.Trim(), @"\s{2,}", " ");

    /// <summary>
    /// Chức năng: xoá kí tự khoảng trắng bị lặp và viết thường tất cả
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ToLowerAndRemoveSpace(this string? text) =>
        RemoveSpaceCharacter(text).ToLower();

    /// <summary>
    /// Chức năng: xoá kí tự khoảng trắng bị lặp và viết hoa tất cả
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ToUpperAndRemoveSpace(this string? text) =>
        RemoveSpaceCharacter(text).ToUpper();

    /// <summary>
    /// Chức năng: loại bỏ toàn bộ kí tự khoảng trắng khỏi chuỗi
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string RemoveAllSpaceChar(this string? text) =>
        string.IsNullOrEmpty(text) ? string.Empty : Regex.Replace(text.Trim(), @"\s+", "");

    #region MySerialize

    private static JsonSerializerOptions _opt = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// Chức năng: sử dụng Deserialize với CamelCase cho đồng bộ toàn hệ thống
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? MyDeserialize<T>(this string? source)
    {
        if (string.IsNullOrEmpty(source))
            return default;

        return JsonSerializer.Deserialize<T>(source, _opt);
    }

    /// <summary>
    /// Chức năng: sử dụng Serialize với CamelCase cho đồng bộ toàn hệ thống
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static string MySerialize<T>(this T source)
    {
        if (source is null)
            return string.Empty;

        return JsonSerializer.Serialize(source, _opt);
    }

    #endregion
}