using PdfSharp.Drawing;

namespace NFEDanfe.Pagination;

/// <summary>Cabeçalho de continuação para páginas adicionais do DANFE.</summary>
internal sealed class DanfeContinuationHeader
{
    private readonly string _nfeNumero;
    private readonly string _serie;
    private readonly int _currentPage;
    private readonly int _totalPages;

    /// <summary>Cria o cabeçalho de continuação.</summary>
    internal DanfeContinuationHeader(string nfeNumero, string serie, int currentPage, int totalPages)
    {
        _nfeNumero = nfeNumero ?? string.Empty;
        _serie = serie ?? string.Empty;
        _currentPage = currentPage;
        _totalPages = totalPages;
    }

    /// <summary>Desenha o cabeçalho de continuação.</summary>
    internal void Draw(XGraphics gfx, DanfeStyleCatalog styles, double x, double y, double width, double height)
    {
        string text = $"NF-e Nº {_nfeNumero}  SÉRIE {_serie}  FOLHA {_currentPage} / {_totalPages}";

        var format = new XStringFormat
        {
            Alignment = XStringAlignment.Center,
            LineAlignment = XLineAlignment.Center
        };

        var rect = new XRect(x, y, width, height);
        gfx.DrawString(text, styles.ValueBoldFont, styles.TextBrush, rect, format);

        gfx.DrawLine(styles.BorderPen, x, y + height, x + width, y + height);
    }
}
