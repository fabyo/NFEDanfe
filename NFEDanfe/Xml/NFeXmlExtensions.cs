using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace NFEDanfe.Xml;

/// <summary>Métodos de extensão internos para facilitar o parsing de XML da NF-e.</summary>
internal static class NFeXmlExtensions
{
    private static readonly XNamespace Ns = "http://www.portalfiscal.inf.br/nfe";

    public static string ElementValue(this XElement parent, string localName)
    {
        return parent.Element(Ns + localName)?.Value ?? string.Empty;
    }

    public static string RequiredElementValue(this XElement parent, string localName, string xpathHint)
    {
        var el = parent.Element(Ns + localName);
        if (el == null)
        {
            throw new DanfeXmlException(xpathHint, $"Elemento '{localName}' não encontrado.");
        }
        return el.Value;
    }

    public static XElement? Child(this XElement parent, string localName)
    {
        return parent.Element(Ns + localName);
    }

    public static XElement RequiredChild(this XElement parent, string localName, string xpathHint)
    {
        var el = parent.Element(Ns + localName);
        if (el == null)
        {
            throw new DanfeXmlException(xpathHint, $"Elemento '{localName}' não encontrado.");
        }
        return el;
    }

    public static string FormatCnpj(this string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        string clean = new string(value.Where(char.IsDigit).ToArray());
        if (clean.Length == 14)
        {
            return $"{clean[..2]}.{clean.Substring(2, 3)}.{clean.Substring(5, 3)}/{clean.Substring(8, 4)}-{clean.Substring(12, 2)}";
        }
        return value;
    }

    public static string FormatCpf(this string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        string clean = new string(value.Where(char.IsDigit).ToArray());
        if (clean.Length == 11)
        {
            return $"{clean[..3]}.{clean.Substring(3, 3)}.{clean.Substring(6, 3)}-{clean.Substring(9, 2)}";
        }
        return value;
    }

    public static string FormatCnpjCpf(this string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        string clean = new string(value.Where(char.IsDigit).ToArray());
        return clean.Length == 14 ? clean.FormatCnpj() : clean.FormatCpf();
    }

    public static string FormatChaveAcesso(this string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length != 44) return value ?? string.Empty;
        var parts = new List<string>();
        for (int i = 0; i < 44; i += 4)
        {
            int len = Math.Min(4, 44 - i);
            parts.Add(value.Substring(i, len));
        }
        return string.Join(" ", parts);
    }

    public static string FormatCep(this string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        string clean = new string(value.Where(char.IsDigit).ToArray());
        if (clean.Length == 8)
        {
            return $"{clean[..5]}-{clean.Substring(5, 3)}";
        }
        return value;
    }
}
