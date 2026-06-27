using PdfSharp.Drawing;
using PdfSharp.Fonts;

namespace NFEDanfe;

/// <summary>Catálogo de estilos visuais do DANFE, instanciado por documento.</summary>
public sealed class DanfeStyleCatalog : IDisposable
{
    private static volatile bool s_fontResolverInitialized;
    private static readonly object s_fontResolverLock = new();

    /// <summary>Fonte do label interno do campo.</summary>
    public XFont LabelFont { get; }

    /// <summary>Fonte regular do valor do campo.</summary>
    public XFont ValueFont { get; }

    /// <summary>Fonte negrito do valor do campo.</summary>
    public XFont ValueBoldFont { get; }

    /// <summary>Caneta para bordas dos campos.</summary>
    public XPen BorderPen { get; }

    /// <summary>Pincel preto para textos.</summary>
    public XSolidBrush TextBrush { get; }

    /// <summary>Pincel vermelho semitransparente para marcas d'água.</summary>
    public XSolidBrush WatermarkBrush { get; }

    /// <summary>Cria o catálogo de estilos a partir da configuração de fontes.</summary>
    public DanfeStyleCatalog(DanfeFontConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);
        EnsureFontResolver(config);

        LabelFont = new XFont(DanfeFontResolver.FamilyName, config.LabelFontSize);
        ValueFont = new XFont(DanfeFontResolver.FamilyName, config.BaseFontSize);
        ValueBoldFont = new XFont(DanfeFontResolver.FamilyName, config.BaseFontSize, XFontStyleEx.Bold);
        BorderPen = new XPen(XColors.Black, 0.75);
        TextBrush = new XSolidBrush(XColors.Black);
        WatermarkBrush = new XSolidBrush(XColor.FromArgb(102, 255, 0, 0));
    }

    /// <summary>Libera recursos do catálogo de estilos.</summary>
    public void Dispose()
    {
        // XFont, XPen e XBrush no PDFsharp são objetos managed leves.
        // IDisposable implementado para conformidade com o padrão de ownership.
    }

    private static void EnsureFontResolver(DanfeFontConfig config)
    {
        if (s_fontResolverInitialized) return;
        lock (s_fontResolverLock)
        {
            if (s_fontResolverInitialized) return;
            GlobalFontSettings.FontResolver = new DanfeFontResolver(config);
            s_fontResolverInitialized = true;
        }
    }
}
