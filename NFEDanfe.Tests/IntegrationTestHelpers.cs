namespace NFEDanfe.Tests;

internal static class IntegrationTestHelpers
{
    public static string FindSampleXml()
    {
        string? current = AppContext.BaseDirectory;

        for (int i = 0; i < 8 && !string.IsNullOrWhiteSpace(current); i++)
        {
            string candidate = Path.Combine(current, "samples", "nota-exemplo.xml");
            if (File.Exists(candidate))
            {
                return candidate;
            }

            current = Directory.GetParent(current)?.FullName;
        }

        throw new FileNotFoundException("Sample XML não encontrado em nenhuma pasta samples/ acessível.");
    }

    public static string FindTestDataXml(string fileName)
    {
        string? current = AppContext.BaseDirectory;

        for (int i = 0; i < 8 && !string.IsNullOrWhiteSpace(current); i++)
        {
            string candidate = Path.Combine(current, "NFEDanfe.Tests", "testdata", fileName);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            // Tenta também diretamente no testdata relativo
            string candidate2 = Path.Combine(current, "testdata", fileName);
            if (File.Exists(candidate2))
            {
                return candidate2;
            }

            current = Directory.GetParent(current)?.FullName;
        }

        throw new FileNotFoundException($"Test data XML '{fileName}' não encontrado.");
    }
}
