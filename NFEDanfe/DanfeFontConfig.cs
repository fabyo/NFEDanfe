namespace NFEDanfe;

/// <summary>Configuração de fontes para renderização do DANFE.</summary>
public sealed class DanfeFontConfig
{
    /// <summary>Cria a configuração usando Arial do sistema ou Roboto como fallback multiplataforma.</summary>
    public DanfeFontConfig()
    {
        (string? regularFontPath, string? boldFontPath) = FindSystemArialPaths();
        regularFontPath ??= FindBundledFontPath("Roboto-Regular.ttf");
        boldFontPath ??= FindBundledFontPath("Roboto-Bold.ttf");
        if (regularFontPath is not null && boldFontPath is not null)
        {
            BaseFontPath = regularFontPath;
            BaseFontBoldPath = boldFontPath;
        }
    }

    private static (string? Regular, string? Bold) FindSystemArialPaths()
    {
        string[][] candidates =
        [
            [Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf", "arialbd.ttf"],
            ["/Library/Fonts", "Arial.ttf", "Arial Bold.ttf"],
            ["/usr/share/fonts/truetype/msttcorefonts", "Arial.ttf", "Arial_Bold.ttf"],
            ["/usr/share/fonts/truetype/msttcorefonts", "arial.ttf", "arialbd.ttf"]
        ];

        foreach (string[] candidate in candidates)
        {
            if (string.IsNullOrWhiteSpace(candidate[0]))
            {
                continue;
            }

            string regularPath = Path.Combine(candidate[0], candidate[1]);
            string boldPath = Path.Combine(candidate[0], candidate[2]);
            if (File.Exists(regularPath) && File.Exists(boldPath))
            {
                return (regularPath, boldPath);
            }
        }

        return (null, null);
    }

    /// <summary>Cria a configuração para uma família distribuída com o pacote.</summary>
    public static DanfeFontConfig FromBundledFont(Options.DanfeFont font)
    {
        string family = font switch
        {
            Options.DanfeFont.Inter => "Inter",
            Options.DanfeFont.Roboto => "Roboto",
            Options.DanfeFont.IbmPlexSans => "IBMPlexSans",
            _ => throw new ArgumentOutOfRangeException(nameof(font), font,
                "A fonte Arial não é distribuída com o pacote. Informe seus arquivos em FontConfig.")
        };

        string regularFileName = $"{family}-Regular.ttf";
        string boldFileName = $"{family}-Bold.ttf";
        string? regularFontPath = FindBundledFontPath(regularFileName);
        string? boldFontPath = FindBundledFontPath(boldFileName);

        if (regularFontPath is null || boldFontPath is null)
        {
            throw new FileNotFoundException(
                $"Os arquivos {regularFileName} e {boldFileName} não foram encontrados na pasta fonts.");
        }

        return new DanfeFontConfig
        {
            BaseFontPath = regularFontPath,
            BaseFontBoldPath = boldFontPath
        };
    }

    /// <summary>Caminho do arquivo .ttf/.otf da fonte regular.</summary>
    public string BaseFontPath { get; set; } = string.Empty;

    /// <summary>Caminho do arquivo .ttf/.otf da fonte negrito.</summary>
    public string BaseFontBoldPath { get; set; } = string.Empty;

    /// <summary>Tamanho base da fonte em pontos.</summary>
    public double BaseFontSize { get; set; } = 9.0;

    /// <summary>Tamanho da fonte do label interno do campo em pontos.</summary>
    public double LabelFontSize { get; set; } = 5.5;

    /// <summary>Valida se os arquivos de fonte existem nos caminhos especificados.</summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(BaseFontPath) || !File.Exists(BaseFontPath))
        {
            throw new InvalidOperationException(
                $"Arquivo de fonte regular não encontrado: '{BaseFontPath}'. " +
                "Verifique se o caminho está correto. Em ambiente Docker, certifique-se de montar " +
                "o volume com as fontes (ex: -v /caminho/local/fonts:/fonts) e apontar para o caminho dentro do container.");
        }

        if (string.IsNullOrWhiteSpace(BaseFontBoldPath) || !File.Exists(BaseFontBoldPath))
        {
            throw new InvalidOperationException(
                $"Arquivo de fonte negrito não encontrado: '{BaseFontBoldPath}'. " +
                "Verifique se o caminho está correto. Em ambiente Docker, certifique-se de montar " +
                "o volume com as fontes (ex: -v /caminho/local/fonts:/fonts) e apontar para o caminho dentro do container.");
        }
    }

    private static string? FindBundledFontPath(string fileName)
    {
        string? current = AppContext.BaseDirectory;
        for (int level = 0; level < 8 && !string.IsNullOrWhiteSpace(current); level++)
        {
            string candidate = Path.Combine(current, "fonts", fileName);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            current = Directory.GetParent(current)?.FullName;
        }

        return null;
    }
}
