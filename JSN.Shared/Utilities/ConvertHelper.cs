using System.Data.SqlTypes;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace JSN.Shared.Utilities;

public static class ConvertHelper
{
    public static Guid ToGuid(object? value, Guid defaultValue = default)
    {
        return Guid.TryParse(value?.ToString(), out var result) ? result : defaultValue;
    }

    public static long ToInt64(object? value, long defaultValue = 0)
    {
        return long.TryParse(value?.ToString(), out var result) ? result : defaultValue;
    }

    public static int ToInt32(object? value, int defaultValue = 0)
    {
        return int.TryParse(value?.ToString(), out var result) ? result : defaultValue;
    }

    public static ushort ToUshort(object? value, ushort defaultValue = 0)
    {
        return ushort.TryParse(value?.ToString(), out var result) ? result : defaultValue;
    }

    public static byte ToByte(object? value, byte defaultValue = 0)
    {
        return byte.TryParse(value?.ToString(), out var result) ? result : defaultValue;
    }

    public static string? ToString(object? value, string? defaultValue = "")
    {
        if (value == null)
        {
            return defaultValue;
        }

        try
        {
            return Convert.ToString(value);
        }
        catch
        {
            return defaultValue;
        }
    }

    public static DateTime ToDateTime(object? value, DateTime defaultValue)
    {
        var result = DateTime.TryParse(value?.ToString(), out var parsedValue) ? parsedValue : defaultValue;

        if (result >= (DateTime)SqlDateTime.MaxValue)
        {
            return (DateTime)SqlDateTime.MaxValue;
        }

        if (result <= (DateTime)SqlDateTime.MinValue)
        {
            return ((DateTime)SqlDateTime.MinValue).AddYears(5);
        }

        return result;
    }

    public static bool ToBoolean(object? value, bool defaultValue = false)
    {
        return bool.TryParse(value?.ToString(), out var result) ? result : defaultValue;
    }

    public static float ToSingle(object? value, float defaultValue = 0)
    {
        return float.TryParse(value?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
            ? result
            : defaultValue;
    }

    public static double ToDouble(object? value, double defaultValue = 0)
    {
        return double.TryParse(value?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
            ? result
            : defaultValue;
    }

    public static decimal ToDecimal(object? value, decimal defaultValue = 0)
    {
        return decimal.TryParse(value?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
            ? result
            : defaultValue;
    }

    public static decimal CurrencyToDecimal(object value)
    {
        var strValue = ToString(value);
        strValue = strValue?.Replace("$", string.Empty);
        strValue = strValue?.Replace(",", string.Empty);
        return ToDecimal(strValue?.Trim());
    }

    public static string ToCurrency(object value)
    {
        return $"{value:C}";
    }

    public static string ToNumeric(object value)
    {
        var result = string.Format(new CultureInfo("en-US"), "{0:#,##0}", value);
        return string.IsNullOrEmpty(result) ? "0" : result;
    }

    public static DateTime ConvertBackFixPrecision(DateTime value)
    {
        const string format = "yyyy-MM-dd HH:mm:ss:fff";
        var stringDate = value.ToString(format);
        return DateTime.ParseExact(stringDate, format, CultureInfo.InvariantCulture);
    }

    public static string ToJson(object obj, bool ignoreNull = false)
    {
        var settings = ignoreNull
            ? new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }
            : null;

        return JsonConvert.SerializeObject(obj, settings);
    }

    [Obsolete("Obsolete")]
    public static long GetInt64HashCode(string text)
    {
        long hashCode = 0;

        if (string.IsNullOrEmpty(text))
        {
            return hashCode;
        }

        var byteContents = Encoding.Unicode.GetBytes(text);

        using SHA256 hash = new SHA256CryptoServiceProvider();
        var hashText = hash.ComputeHash(byteContents);
        var hashCodeStart = BitConverter.ToInt64(hashText, 0);
        var hashCodeMedium = BitConverter.ToInt64(hashText, 8);
        var hashCodeEnd = BitConverter.ToInt64(hashText, 24);
        hashCode = hashCodeStart ^ hashCodeMedium ^ hashCodeEnd;

        return hashCode;
    }
}