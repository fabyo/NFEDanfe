using PdfSharp.Fonts;

namespace NFEDanfe;

/// <summary>Resolvedor interno de fontes que carrega arquivos .ttf/.otf do disco.</summary>
internal sealed class DanfeFontResolver : IFontResolver
{
    private const string RegularFaceName = "DanfeRegular";
    private const string BoldFaceName = "DanfeBold";

    internal const string FamilyName = "DanfeFont";

    private readonly byte[] _regularBytes;
    private readonly byte[] _boldBytes;

    internal DanfeFontResolver(DanfeFontConfig config)
    {
        _regularBytes = File.ReadAllBytes(config.BaseFontPath);
        _boldBytes = File.ReadAllBytes(config.BaseFontBoldPath);
    }

    public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        if (!string.Equals(familyName, FamilyName, StringComparison.OrdinalIgnoreCase))
            return null;

        return new FontResolverInfo(isBold ? BoldFaceName : RegularFaceName);
    }

    public byte[]? GetFont(string faceName)
    {
        return faceName switch
        {
            RegularFaceName => _regularBytes,
            BoldFaceName => _boldBytes,
            _ => null
        };
    }
}
