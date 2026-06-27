namespace NFEDanfe;

/// <summary>Configuração de fontes para renderização do DANFE.</summary>
public sealed class DanfeFontConfig
{
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
}
