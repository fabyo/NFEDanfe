using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace NFEDanfe;

/// <summary>Motor principal de renderização do DANFE.</summary>
public sealed class DanfeEngine : IDisposable
{
    private const double MmToPt = 2.834645;
    private const double PageWidthPt = 595.27;
    private const double PageHeightPt = 841.89;

    private readonly PdfDocument _document;
    private readonly DanfePageMode _mode;

    /// <summary>Catálogo de estilos visuais do documento.</summary>
    public DanfeStyleCatalog Styles { get; }

    /// <summary>Largura útil da página em pontos, já descontadas as margens.</summary>
    public double UsableWidth { get; }

    /// <summary>Altura útil da página em pontos, já descontadas as margens.</summary>
    public double UsableHeight { get; }

    /// <summary>Margem esquerda em pontos.</summary>
    internal double MarginLeftPt { get; }

    /// <summary>Margem superior em pontos.</summary>
    internal double MarginTopPt { get; }

    /// <summary>Cria o motor de renderização do DANFE.</summary>
    public DanfeEngine(DanfeFontConfig fontConfig, DanfeMargins margins, DanfePageMode mode)
    {
        ArgumentNullException.ThrowIfNull(fontConfig);
        ArgumentNullException.ThrowIfNull(margins);

        fontConfig.Validate();

        _document = new PdfDocument();
        _mode = mode;

        Styles = new DanfeStyleCatalog(fontConfig);

        double pageW = mode == DanfePageMode.Portrait ? PageWidthPt : PageHeightPt;
        double pageH = mode == DanfePageMode.Portrait ? PageHeightPt : PageWidthPt;

        MarginLeftPt = margins.Left * MmToPt;
        MarginTopPt = margins.Top * MmToPt;

        UsableWidth = pageW - (margins.Left * MmToPt) - (margins.Right * MmToPt);
        UsableHeight = pageH - (margins.Top * MmToPt) - (margins.Bottom * MmToPt);
    }

    /// <summary>Adiciona uma nova página e retorna o contexto gráfico.</summary>
    public XGraphics BeginPage()
    {
        var page = _document.AddPage();
        if (_mode == DanfePageMode.Landscape)
        {
            page.Width = XUnit.FromPoint(PageHeightPt);
            page.Height = XUnit.FromPoint(PageWidthPt);
        }
        else
        {
            page.Width = XUnit.FromPoint(PageWidthPt);
            page.Height = XUnit.FromPoint(PageHeightPt);
        }

        return XGraphics.FromPdfPage(page);
    }

    /// <summary>Finaliza o documento e retorna os bytes do PDF.</summary>
    public byte[] Build()
    {
        using var stream = new MemoryStream();
        Build(stream);
        return stream.ToArray();
    }

    /// <summary>Finaliza o documento e salva no stream fornecido.</summary>
    public void Build(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        _document.Save(stream, false);
    }

    /// <summary>Libera os recursos do motor e do documento PDF.</summary>
    public void Dispose()
    {
        Styles.Dispose();
        _document.Dispose();
    }
}
