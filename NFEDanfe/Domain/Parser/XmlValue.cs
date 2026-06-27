using System;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace NFEDanfe.Domain.Parser;

/// <summary>Métodos auxiliares para extração e decodificação de valores XML.</summary>
internal static class XmlValue
{
    private static readonly Regex NcrRegex = new(@"&?#([0-9]+);", RegexOptions.Compiled);
    private static readonly Regex HexNcrRegex = new(@"&?#x([0-9a-fA-F]+);", RegexOptions.Compiled);

    public static string Text(this XElement? element, XNamespace ns, string name, string fallback = "")
    {
        string val = element?.Element(ns + name)?.Value?.Trim() ?? fallback;
        return DecodeText(val);
    }

    private static string DecodeText(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        string decoded = WebUtility.HtmlDecode(input);

        decoded = NcrRegex.Replace(decoded, match =>
        {
            if (int.TryParse(match.Groups[1].Value, out int code))
            {
                return ((char)code).ToString();
            }
            return match.Value;
        });

        decoded = HexNcrRegex.Replace(decoded, match =>
        {
            try
            {
                int code = Convert.ToInt32(match.Groups[1].Value, 16);
                return ((char)code).ToString();
            }
            catch
            {
                return match.Value;
            }
        });

        return decoded;
    }

    public static decimal Decimal(this XElement? element, XNamespace ns, string name, decimal fallback = 0)
    {
        string? value = element?.Element(ns + name)?.Value;
        return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal result)
            ? result
            : fallback;
    }

    public static decimal? NullableDecimal(this XElement? element, XNamespace ns, string name)
    {
        string? value = element?.Element(ns + name)?.Value;
        return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal result)
            ? result
            : null;
    }

    public static int Int(this XElement? element, XNamespace ns, string name, int fallback = 0)
    {
        string? value = element?.Element(ns + name)?.Value;
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result) ? result : fallback;
    }

    public static DateTime DateTime(this XElement? element, XNamespace ns, string name)
    {
        string? value = element?.Element(ns + name)?.Value;
        return System.DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out System.DateTime result)
            ? result
            : System.DateTime.MinValue;
    }

    public static DateTime? NullableDateTime(this XElement? element, XNamespace ns, string name)
    {
        string? value = element?.Element(ns + name)?.Value;
        return System.DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out System.DateTime result)
            ? result
            : null;
    }
}
