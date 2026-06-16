using System.Globalization;
using System.Xml.Linq;

namespace NFEDanfe.Domain.Parser;

internal static class XmlValue
{
    public static string Text(this XElement? element, XNamespace ns, string name, string fallback = "")
    {
        return element?.Element(ns + name)?.Value?.Trim() ?? fallback;
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
