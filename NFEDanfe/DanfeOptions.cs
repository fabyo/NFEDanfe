namespace NFEDanfe;

public enum DanfeFont
{
    Arial,
    Inter,
    Roboto,
    IbmPlexSans
}

public sealed record DanfeOptions
{
    public static DanfeOptions Default { get; } = new();

    public byte[]? LogoBytes { get; init; }
    public bool ValidateBeforeGenerate { get; init; } = true;
    public int? TipoImpressaoOverride { get; init; }
    public bool? CanceledOverride { get; init; }
    public bool EmitFooter { get; init; } = true;
    public DanfeFont Font { get; init; } = DanfeFont.Arial;
    public string? CustomFontName { get; init; }
    public System.Text.Encoding? CustomXmlEncoding { get; init; }

    public string FontName => CustomFontName ?? Font switch
    {
        DanfeFont.Arial => "Arial",
        DanfeFont.Inter => "Inter",
        DanfeFont.Roboto => "Roboto",
        DanfeFont.IbmPlexSans => "IBM Plex Sans",
        _ => "Arial"
    };
}
