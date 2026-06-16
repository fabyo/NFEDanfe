using NFEDanfe.Domain.Formatting;

namespace NFEDanfe.Domain.Validation;

internal static class NFeAccessKeyValidator
{
    public static bool IsValid(string? value)
    {
        string key = DocumentFormatter.OnlyDigits(value);

        if (key.Length != 44)
        {
            return false;
        }

        if (key.Substring(20, 2) != "55")
        {
            return false;
        }

        int expectedDigit = CalculateDigit(key[..43]);
        int actualDigit = key[43] - '0';

        return expectedDigit == actualDigit;
    }

    private static int CalculateDigit(string first43Digits)
    {
        int weight = 2;
        int sum = 0;

        for (int i = first43Digits.Length - 1; i >= 0; i--)
        {
            sum += (first43Digits[i] - '0') * weight;
            weight = weight == 9 ? 2 : weight + 1;
        }

        int remainder = sum % 11;
        int digit = 11 - remainder;
        return digit is 10 or 11 ? 0 : digit;
    }
}
