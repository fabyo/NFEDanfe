namespace NFEDanfe.Tests;

internal static class IntegrationTestHelpers
{
    public static string FindSampleXml()
    {
        string? current = AppContext.BaseDirectory;

        for (int i = 0; i < 8 && !string.IsNullOrWhiteSpace(current); i++)
        {
            string candidate = Path.Combine(current, "samples", "nota-exemplo-procNFe.xml");
            if (File.Exists(candidate))
            {
                return candidate;
            }

            current = Directory.GetParent(current)?.FullName;
        }

        throw new FileNotFoundException("Sample XML não encontrado em nenhuma pasta samples/ acessível.");
    }
}
