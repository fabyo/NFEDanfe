using System;
using System.Linq;
using PdfSharp.Drawing;
using NFEDanfe.Domain.Models;
using NFEDanfe.Domain.Formatting;

namespace NFEDanfe.Blocks;

/// <summary>Bloco de fatura/duplicatas.</summary>
internal sealed class InvoiceBlock
{
    private static double Mm(double mm) => mm * 2.834645;

    private readonly DanfeModel _model;

    internal InvoiceBlock(DanfeModel model)
    {
        _model = model;
    }

    /// <summary>Retorna true se o bloco deve ser renderizado.</summary>
    internal bool ShouldRender => _model.Cobranca != null;

    /// <summary>Desenha o bloco de fatura. Retorna a altura consumida.</summary>
    internal double Draw(XGraphics gfx, DanfeStyleCatalog styles, double x, double y, double width)
    {
        if (!ShouldRender) return 0;

        double titleH = 12.0;
        double contentH = 28.0;
        double height = titleH + contentH;

        // 1. Título "FATURA / DUPLICATAS" com fundo cinza claro e borda
        var grayBrush = new XSolidBrush(XColor.FromArgb(224, 224, 224));
        gfx.DrawRectangle(styles.BorderPen, grayBrush, x, y, width, titleH);

        var titleFormat = new XStringFormat { Alignment = XStringAlignment.Near, LineAlignment = XLineAlignment.Center };
        var titleFont = new XFont(DanfeFontResolver.FamilyName, 6.0, XFontStyleEx.Bold);
        gfx.DrawString("FATURA / DUPLICATAS", titleFont, styles.TextBrush,
            new XRect(x + 4, y, width - 8, titleH), titleFormat);
        y += titleH;

        // Borda externa da área de conteúdo das duplicatas
        gfx.DrawRectangle(styles.BorderPen, x, y, width, contentH);

        var duplicatas = _model.Cobranca!.Duplicatas.ToList();
        if (duplicatas.Any())
        {
            double cardW = 90.0;
            double cardH = 22.0;
            double cardSpacing = 4.0;
            double currentX = x + 4;
            double cardY = y + (contentH - cardH) / 2;

            int maxCards = (int)Math.Floor((width - 8) / (cardW + cardSpacing));
            int cardsToDraw = Math.Min(duplicatas.Count, maxCards);

            for (int i = 0; i < cardsToDraw; i++)
            {
                var dup = duplicatas[i];
                gfx.DrawRectangle(styles.BorderPen, currentX, cardY, cardW, cardH);

                // Linhas internas do card
                double rowH = cardH / 3.0;

                var labelFont = new XFont(DanfeFontResolver.FamilyName, 5.0, XFontStyleEx.Regular);
                var valueFont = new XFont(DanfeFontResolver.FamilyName, 6.0, XFontStyleEx.Bold);
                var leftFormat = new XStringFormat { Alignment = XStringAlignment.Near, LineAlignment = XLineAlignment.Center };
                var rightFormat = new XStringFormat { Alignment = XStringAlignment.Far, LineAlignment = XLineAlignment.Center };

                // Linha 1: DUP: e Numero
                gfx.DrawString("DUP:", labelFont, styles.TextBrush, new XRect(currentX + 3, cardY, cardW - 6, rowH), leftFormat);
                gfx.DrawString(dup.Numero, valueFont, styles.TextBrush, new XRect(currentX + 3, cardY, cardW - 6, rowH), rightFormat);

                // Linha 2: VENC: e Vencimento
                string venc = dup.Vencimento == DateTime.MinValue ? "" : dup.Vencimento.ToString("dd/MM/yyyy");
                gfx.DrawString("VENC:", labelFont, styles.TextBrush, new XRect(currentX + 3, cardY + rowH, cardW - 6, rowH), leftFormat);
                gfx.DrawString(venc, valueFont, styles.TextBrush, new XRect(currentX + 3, cardY + rowH, cardW - 6, rowH), rightFormat);

                // Linha 3: VALOR: e Valor
                string valStr = $"R$ {DocumentFormatter.Money(dup.Valor)}";
                gfx.DrawString("VALOR:", labelFont, styles.TextBrush, new XRect(currentX + 3, cardY + rowH * 2, cardW - 6, rowH), leftFormat);
                gfx.DrawString(valStr, valueFont, styles.TextBrush, new XRect(currentX + 3, cardY + rowH * 2, cardW - 6, rowH), rightFormat);

                currentX += cardW + cardSpacing;
            }
        }
        else
        {
            var noDupFont = new XFont(DanfeFontResolver.FamilyName, 6.0, XFontStyleEx.Regular);
            var centerFormat = new XStringFormat { Alignment = XStringAlignment.Center, LineAlignment = XLineAlignment.Center };
            gfx.DrawString("Sem duplicatas informadas", noDupFont, styles.TextBrush,
                new XRect(x, y, width, contentH), centerFormat);
        }

        return height;
    }
}
