using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;

namespace NFEDanfe.Builder;

/// <summary>Componente atômico de renderização de um campo do DANFE.</summary>
public sealed class DanfeField
{
    private const double PaddingLeft = 1.5;
    private const double PaddingTop = 1.0;

    /// <summary>Texto do label do campo.</summary>
    public string Label { get; }

    /// <summary>Valor do campo.</summary>
    public string Value { get; }

    /// <summary>Percentual de largura do campo na linha.</summary>
    public double WidthPct { get; }

    /// <summary>Alinhamento do valor do campo.</summary>
    public XStringAlignment ValueAlignment { get; set; } = XStringAlignment.Near;

    /// <summary>Cria um campo com label, valor e percentual de largura.</summary>
    public DanfeField(string label, string value, double widthPct, XStringAlignment valueAlignment = XStringAlignment.Near)
    {
        Label = label ?? string.Empty;
        Value = value ?? string.Empty;
        WidthPct = widthPct;
        ValueAlignment = valueAlignment;
    }

    /// <summary>Calcula a altura padrão do campo em pontos.</summary>
    internal static double DefaultHeight(DanfeStyleCatalog styles)
    {
        return 25.0; // Altura ideal para suportar label de 2 linhas + valor sem nenhuma sobreposição
    }

    internal void Draw(XGraphics gfx, DanfeStyleCatalog styles, double x, double y, double width, double height, XFont? labelFontOverride = null, XFont? valueFontOverride = null)
    {
        gfx.DrawRectangle(styles.BorderPen, x, y, width, height);

        double availableWidth = width - (PaddingLeft * 2);

        // Desenhar label com quebra de linha automática usando XTextFormatter no topo
        // Usamos altura de 13.0 pt para garantir que caibam 2 linhas de fonte 5.5 pt
        var tf = new XTextFormatter(gfx);
        var labelRect = new XRect(x + PaddingLeft, y + PaddingTop, availableWidth, 13.0);
        var labelFont = labelFontOverride ?? styles.LabelFont;
        tf.DrawString(Label.ToUpper(), labelFont, styles.TextBrush, labelRect);

        // Desenhar valor posicionado e alinhado na base do campo
        var valFont = valueFontOverride ?? styles.ValueFont;
        double valueH = valFont.Size + 1.0;
        double valueY = y + height - valueH - PaddingTop - 0.5;
        var valueFormat = new XStringFormat
        {
            Alignment = ValueAlignment,
            LineAlignment = XLineAlignment.Center
        };
        string truncatedValue = TruncateText(gfx, Value, valFont, availableWidth);

        var valueRect = new XRect(x + PaddingLeft, valueY, availableWidth, valueH);
        gfx.DrawString(truncatedValue, valFont, styles.TextBrush, valueRect, valueFormat);
    }

    /// <summary>Trunca texto com reticências se exceder a largura disponível.</summary>
    internal static string TruncateText(XGraphics gfx, string text, XFont font, double maxWidth)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;

        double textWidth = gfx.MeasureString(text, font).Width;
        if (textWidth <= maxWidth) return text;

        const string ellipsis = "\u2026";
        double ellipsisWidth = gfx.MeasureString(ellipsis, font).Width;
        double targetWidth = maxWidth - ellipsisWidth;

        if (targetWidth <= 0) return ellipsis;

        int low = 0;
        int high = text.Length;
        while (low < high)
        {
            int mid = (low + high + 1) / 2;
            double w = gfx.MeasureString(text[..mid], font).Width;
            if (w <= targetWidth)
                low = mid;
            else
                high = mid - 1;
        }

        return low > 0 ? text[..low] + ellipsis : ellipsis;
    }
}
