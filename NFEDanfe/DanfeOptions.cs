namespace NFEDanfe;

public sealed record DanfeOptions
{
    public static DanfeOptions Default { get; } = new();

    public byte[]? LogoBytes { get; init; }
    public bool ValidateBeforeGenerate { get; init; } = true;
    public int? TipoImpressaoOverride { get; init; }
    public bool EmitFooter { get; init; } = true;
}
