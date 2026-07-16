using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace NFEDanfe.Domain.Formatting;

/// <summary>Normaliza textos livres exibidos no bloco de dados adicionais.</summary>
internal static class AdditionalDataText
{
    private static readonly TimeSpan RegexTimeout = TimeSpan.FromMilliseconds(100);
    private const RegexOptions CommonOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase;

    private static readonly Regex ScriptOrStyleRegex = new(
        @"<\s*(script|style)\b[^>]*>.*?<\s*/\s*\1\s*>",
        CommonOptions | RegexOptions.Singleline,
        RegexTimeout);

    private static readonly Regex BreakTagRegex = new(
        @"<\s*br\b[^>]*>",
        CommonOptions,
        RegexTimeout);

    private static readonly Regex MalformedBreakTagRegex = new(
        @"(?:e?lt;)\s*br\b[^;\r\n]{0,100}?(?:e?gt;)",
        CommonOptions,
        RegexTimeout);

    private static readonly Regex BlockTagRegex = new(
        @"<\s*/?\s*(?:address|blockquote|div|h[1-6]|li|ol|p|table|tr|ul)\b[^>]*>",
        CommonOptions,
        RegexTimeout);

    private static readonly Regex HtmlTagRegex = new(
        @"<[^>\r\n]{1,500}>",
        CommonOptions,
        RegexTimeout);

    private static readonly Regex MalformedHtmlTagRegex = new(
        @"(?:e?lt;)\s*/?\s*[a-z][^;\r\n]{0,200}?(?:e?gt;)",
        CommonOptions,
        RegexTimeout);

    private static readonly Regex HorizontalWhitespaceRegex = new(
        @"[\p{Zs}\t\f\v]+",
        RegexOptions.Compiled | RegexOptions.CultureInvariant,
        RegexTimeout);

    private static readonly Regex NewlinePaddingRegex = new(
        @"[\p{Zs}\t\f\v]*\n[\p{Zs}\t\f\v]*",
        RegexOptions.Compiled | RegexOptions.CultureInvariant,
        RegexTimeout);

    private static readonly Regex RepeatedNewlineRegex = new(
        @"\n{2,}",
        RegexOptions.Compiled | RegexOptions.CultureInvariant,
        RegexTimeout);

    internal static string Normalize(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        string text = DecodeHtmlEntities(input);

        text = ScriptOrStyleRegex.Replace(text, string.Empty);
        text = MalformedBreakTagRegex.Replace(text, "\n");
        text = BreakTagRegex.Replace(text, "\n");
        text = BlockTagRegex.Replace(text, "\n");
        text = MalformedHtmlTagRegex.Replace(text, string.Empty);
        text = HtmlTagRegex.Replace(text, string.Empty);

        text = text
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n')
            .Replace(';', '\n');

        text = RemoveUnsupportedControlCharacters(text);
        text = HorizontalWhitespaceRegex.Replace(text, " ");
        text = NewlinePaddingRegex.Replace(text, "\n");
        text = RepeatedNewlineRegex.Replace(text, "\n");

        return text.Trim();
    }

    internal static string? NormalizeNullable(string? input)
    {
        return input is null ? null : Normalize(input);
    }

    private static string DecodeHtmlEntities(string input)
    {
        string decoded = input;

        for (int pass = 0; pass < 3; pass++)
        {
            string next = WebUtility.HtmlDecode(decoded);
            if (string.Equals(next, decoded, StringComparison.Ordinal))
            {
                break;
            }

            decoded = next;
        }

        return decoded;
    }

    private static string RemoveUnsupportedControlCharacters(string text)
    {
        var builder = new StringBuilder(text.Length);

        foreach (char character in text)
        {
            if (character == '\n' || !char.IsControl(character))
            {
                builder.Append(character);
            }
        }

        return builder.ToString();
    }
}
