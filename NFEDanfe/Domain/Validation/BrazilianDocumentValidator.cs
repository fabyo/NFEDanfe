using NFEDanfe.Domain.Formatting;

namespace NFEDanfe.Domain.Validation;

internal static class BrazilianDocumentValidator
{
    private static readonly int[] CnpjFirstWeights = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
    private static readonly int[] CnpjSecondWeights = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
    private static readonly int[] CpfFirstWeights = [10, 9, 8, 7, 6, 5, 4, 3, 2];
    private static readonly int[] CpfSecondWeights = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];

    public static bool IsValidCnpj(string? value)
    {
        string cnpj = DocumentFormatter.OnlyLettersAndDigits(value);

        if (cnpj.Length != 14 || !cnpj[^2..].All(char.IsDigit) || cnpj.Distinct().Count() == 1)
        {
            return false;
        }

        int firstDigit = CalculateCnpjDigit(cnpj[..12], CnpjFirstWeights);
        int secondDigit = CalculateCnpjDigit(cnpj[..12] + firstDigit, CnpjSecondWeights);

        return cnpj[^2..] == $"{firstDigit}{secondDigit}";
    }

    public static bool IsValidCpf(string? value)
    {
        string cpf = DocumentFormatter.OnlyDigits(value);

        if (cpf.Length != 11 || cpf.Distinct().Count() == 1)
        {
            return false;
        }

        int firstDigit = CalculateCpfDigit(cpf[..9], CpfFirstWeights);
        int secondDigit = CalculateCpfDigit(cpf[..9] + firstDigit, CpfSecondWeights);

        return cpf[^2..] == $"{firstDigit}{secondDigit}";
    }

    public static bool IsValidCnpjOrCpf(string? value)
    {
        string normalized = DocumentFormatter.OnlyLettersAndDigits(value);

        return normalized.Length switch
        {
            11 => IsValidCpf(normalized),
            14 => IsValidCnpj(normalized),
            _ => false
        };
    }

    private static int CalculateCnpjDigit(string value, IReadOnlyList<int> weights)
    {
        int sum = 0;

        for (int i = 0; i < weights.Count; i++)
        {
            sum += ToCnpjValue(value[i]) * weights[i];
        }

        int remainder = sum % 11;
        return remainder < 2 ? 0 : 11 - remainder;
    }

    private static int CalculateCpfDigit(string value, IReadOnlyList<int> weights)
    {
        int sum = 0;

        for (int i = 0; i < weights.Count; i++)
        {
            sum += (value[i] - '0') * weights[i];
        }

        int remainder = sum % 11;
        return remainder < 2 ? 0 : 11 - remainder;
    }

    private static int ToCnpjValue(char value)
    {
        return value switch
        {
            >= '0' and <= '9' => value - '0',
            >= 'A' and <= 'Z' => value - 48,
            _ => throw new ArgumentOutOfRangeException(nameof(value), $"Caractere inválido para CNPJ: {value}")
        };
    }
}
