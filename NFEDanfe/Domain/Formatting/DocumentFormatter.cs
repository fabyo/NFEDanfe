using System.Globalization;
using System.Text.RegularExpressions;
using NFEDanfe.Domain.Models;

namespace NFEDanfe.Domain.Formatting;

public static class DocumentFormatter
{
    public static readonly CultureInfo Brazil = CultureInfo.GetCultureInfo("pt-BR");

    public static string OnlyDigits(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : Regex.Replace(value, @"[^\d]", "");
    }

    public static string OnlyLettersAndDigits(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : Regex.Replace(value.ToUpperInvariant(), @"[^0-9A-Z]", "");
    }

    public static string CnpjCpf(string? value)
    {
        string document = OnlyLettersAndDigits(value);

        return document.Length switch
        {
            11 when document.All(char.IsDigit) => $"{document[..3]}.{document.Substring(3, 3)}.{document.Substring(6, 3)}-{document.Substring(9, 2)}",
            14 => $"{document[..2]}.{document.Substring(2, 3)}.{document.Substring(5, 3)}/{document.Substring(8, 4)}-{document.Substring(12, 2)}",
            _ => value ?? string.Empty
        };
    }

    public static string Cep(string? value)
    {
        string digits = OnlyDigits(value);
        return digits.Length == 8 ? $"{digits[..5]}-{digits.Substring(5, 3)}" : value ?? string.Empty;
    }

    public static string Phone(string? value)
    {
        string digits = OnlyDigits(value);

        return digits.Length switch
        {
            10 => $"({digits[..2]}) {digits.Substring(2, 4)}-{digits.Substring(6, 4)}",
            11 => $"({digits[..2]}) {digits.Substring(2, 5)}-{digits.Substring(7, 4)}",
            _ => value ?? string.Empty
        };
    }

    public static string Money(decimal value)
    {
        return value.ToString("N2", Brazil);
    }

    public static string Decimal(decimal? value, int decimals = 3)
    {
        return value.HasValue ? value.Value.ToString($"N{decimals}", Brazil) : string.Empty;
    }

    public static string Address(Endereco endereco)
    {
        string text = $"{endereco.Logradouro}, {endereco.Numero}";

        if (!string.IsNullOrWhiteSpace(endereco.Complemento))
        {
            text += $" - {endereco.Complemento}";
        }

        return text;
    }

    public static string FullAddress(Endereco endereco)
    {
        string text = Address(endereco);
        text += $", {endereco.Bairro}, {endereco.Municipio}-{endereco.Uf}";
        return text;
    }
}
