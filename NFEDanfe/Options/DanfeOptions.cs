using System.Text;

namespace NFEDanfe.Options;

/// <summary>Configuração global do documento DANFE.</summary>
public sealed record DanfeOptions
{
    /// <summary>Configuração padrão do DANFE.</summary>
    public static DanfeOptions Default { get; } = new();

    /// <summary>Ambiente de emissão.</summary>
    public DanfeAmbiente Ambiente { get; set; } = DanfeAmbiente.Producao;

    /// <summary>Caminho do logotipo do emitente (null para omitir).</summary>
    public string? LogoPath { get; set; }

    /// <summary>Bytes do logotipo do emitente (null para omitir).</summary>
    public byte[]? LogoBytes { get; set; }

    /// <summary>Modo de impressão (Portrait ou Landscape).</summary>
    public DanfePageMode PageMode { get; set; } = DanfePageMode.Portrait;

    /// <summary>Configuração de fontes.</summary>
    public DanfeFontConfig FontConfig { get; set; } = new();

    /// <summary>Margens do documento.</summary>
    public DanfeMargins Margins { get; set; } = new(5);

    /// <summary>Valida o modelo antes de gerar.</summary>
    public bool ValidateBeforeGenerate { get; set; } = true;

    /// <summary>Sobrescrita para tipo de impressão (1 = Retrato, 2 = Paisagem).</summary>
    public int? TipoImpressaoOverride { get; set; }

    /// <summary>Sobrescrita de nota cancelada.</summary>
    public bool? CanceledOverride { get; set; }

    /// <summary>Emitir rodapé da página.</summary>
    public bool EmitFooter { get; set; } = true;

    /// <summary>Fonte padrão a ser utilizada.</summary>
    public DanfeFont Font { get; set; } = DanfeFont.Arial;

    /// <summary>Nome de fonte customizada.</summary>
    public string? CustomFontName { get; set; }

    /// <summary>Encoding customizado para leitura do XML.</summary>
    public Encoding? CustomXmlEncoding { get; set; }

    /// <summary>Retorna o nome da fonte a ser utilizado.</summary>
    public string FontName => CustomFontName ?? Font switch
    {
        DanfeFont.Arial => "Arial",
        DanfeFont.Inter => "Inter",
        DanfeFont.Roboto => "Roboto",
        DanfeFont.IbmPlexSans => "IBM Plex Sans",
        DanfeFont.JetBrainsMono => "JetBrains Mono",
        _ => "Arial"
    };
}
