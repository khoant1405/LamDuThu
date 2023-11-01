using System.Data.SqlTypes;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace JSN.Shared.Utilities;

public static class ConvertHelper
{
    public static Guid ToGuid(object val, Guid defValue)
    {
        Guid ret;
        try
        {
            ret = new Guid(val.ToString());
        }
        catch
        {
            ret = defValue;
        }

        return ret;
    }

    public static Guid ToGuid(object val)
    {
        return ToGuid(val, Guid.Empty);
    }

    public static Guid ToGuid(SqlGuid val)
    {
        return val.IsNull ? Guid.Empty : val.Value;
    }

    public static long ToInt64(object obj, long defaultValue = 0)
    {
        return long.TryParse(obj?.ToString(), out var parsedVal) ? parsedVal : defaultValue;
    }

    public static int ToInt32(object obj, int defaultValue = 0)
    {
        return int.TryParse(obj?.ToString(), out var parsedVal) ? parsedVal : defaultValue;
    }

    public static ushort ToUshort(object obj, ushort defaultValue = 0)
    {
        return ushort.TryParse(obj?.ToString(), out var parsedVal) ? parsedVal : defaultValue;
    }

    public static byte ToByte(object obj, byte defaultValue = 0)
    {
        return byte.TryParse(obj?.ToString(), out var parsedVal) ? parsedVal : defaultValue;
    }

    public static string ToString(object obj, string defaultValue = "")
    {
        if (obj == null)
        {
            return defaultValue;
        }

        string retVal;
        try
        {
            retVal = Convert.ToString(obj);
        }
        catch
        {
            retVal = defaultValue;
        }

        return retVal;
    }

    public static DateTime ToDateTime(object obj, DateTime defaultValue)
    {
        var retVal = DateTime.TryParse(obj?.ToString(), out var parsedVal) ? parsedVal : defaultValue;
        if (retVal >= (DateTime)SqlDateTime.MaxValue)
        {
            return (DateTime)SqlDateTime.MaxValue;
        }

        return retVal <= (DateTime)SqlDateTime.MinValue ? ((DateTime)SqlDateTime.MinValue).AddYears(5) : retVal;
    }

    public static DateTime ToDateTime(object obj)
    {
        return ToDateTime(obj, DateTime.Now);
    }

    public static bool ToBoolean(object obj, bool defaultValue = false)
    {
        return bool.TryParse(obj?.ToString(), out var parsedVal) ? parsedVal : defaultValue;
    }

    public static float ToSingle(object obj, float defaultValue = 0)
    {
        return float.TryParse(obj?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedVal)
            ? parsedVal
            : defaultValue;
    }

    public static double ToDouble(object obj, double defaultValue = 0)
    {
        return double.TryParse(obj?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedVal)
            ? parsedVal
            : defaultValue;
    }

    public static decimal ToDecimal(object obj, decimal defaultValue = 0)
    {
        return decimal.TryParse(obj?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedVal)
            ? parsedVal
            : defaultValue;
    }

    public static decimal ToDecimal(SqlDecimal val)
    {
        return val.IsNull ? decimal.Zero : val.Value;
    }

    public static long ToJavaScriptMilliseconds(DateTime dt)
    {
        var datetimeMinTimeTicks = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
        return (dt.ToUniversalTime().Ticks - datetimeMinTimeTicks) / 10000;
    }

    public static decimal CurrencyToDecimal(object val)
    {
        var strVal = ToString(val);
        strVal = strVal.Replace("$", string.Empty);
        strVal = strVal.Replace(",", string.Empty);
        return ToDecimal(strVal.Trim());
    }

    /// <summary>
    ///     Format to $0.00
    /// </summary>
    /// <param name="val">Value to format</param>
    /// <returns></returns>
    public static string ToCurrency(object val)
    {
        return $"{val:C}";
    }

    /// <summary>
    ///     Format to 0.00
    /// </summary>
    /// <param name="val">Value to format</param>
    /// <returns></returns>
    public static string ToNumeric(object val)
    {
        var result = string.Format(new CultureInfo("en-US"), "{0:#,##0}", val);
        return string.IsNullOrEmpty(result) ? "0" : result;
    }

    public static DateTime ConvertBackFixPrecision(DateTime val)
    {
        const string format = "yyyy-MM-dd HH:mm:ss:fff";
        var stringDate = val.ToString(format);
        return DateTime.ParseExact(stringDate, format, CultureInfo.InvariantCulture);
    }

    /// <summary>
    ///     Convert object to JSON
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="isIgnoreNull"></param>
    /// <returns></returns>
    public static string ToJson(object obj, bool isIgnoreNull = false)
    {
        if (isIgnoreNull)
        {
            return JsonConvert.SerializeObject(obj,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        return JsonConvert.SerializeObject(obj);
    }

    public static long GetInt64HashCode(string strText)
    {
        long hashCode = 0;
        if (string.IsNullOrEmpty(strText))
        {
            return hashCode;
        }

        //Unicode Encode Covering all characterset
        var byteContents = Encoding.Unicode.GetBytes(strText);
        SHA256 hash =
            new SHA256CryptoServiceProvider();
        var hashText = hash.ComputeHash(byteContents);
        //32Byte hashText separate
        //hashCodeStart = 0~7  8Byte
        //hashCodeMedium = 8~23  8Byte
        //hashCodeEnd = 24~31  8Byte
        //and Fold
        var hashCodeStart = BitConverter.ToInt64(hashText, 0);
        var hashCodeMedium = BitConverter.ToInt64(hashText, 8);
        var hashCodeEnd = BitConverter.ToInt64(hashText, 24);
        hashCode = hashCodeStart ^ hashCodeMedium ^ hashCodeEnd;
        return hashCode;
    }
}