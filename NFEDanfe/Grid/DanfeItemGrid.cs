using System;
using System.Collections.Generic;
using System.Linq;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using NFEDanfe.Builder;
using NFEDanfe.Pagination;

namespace NFEDanfe.Grid;

/// <summary>Componente de renderização da grade de itens do DANFE com suporte a alturas dinâmicas e texto de descrição multilinhas.</summary>
internal sealed class DanfeItemGrid
{
    /// <summary>Desenha a grade de itens para uma fatia específica e retorna o Y final.</summary>
    internal double Draw(
        XGraphics gfx,
        DanfeStyleCatalog styles,
        DanfeItemPageSlice slice,
        IReadOnlyList<DanfeItemRow> allItems,
        IReadOnlyList<double> rowHeights, // Alturas individuais pré-calculadas para cada item
        IReadOnlyList<DanfeItemColumn> columns,
        double x, double y,
        double availableWidth,
        bool drawColumnHeaders)
    {
        double currentY = y;
        double[] colWidths = GetColumnWidths(availableWidth, columns);
        
        var solidPen = new XPen(XColors.Black, 0.4); // Borda externa e cabeçalhos sólidos
        var dashedPen = new XPen(XColors.Black, 0.4)
        {
            DashStyle = XDashStyle.Custom,
            DashPattern = new double[] { 2.0, 2.0 } // Tracejado [2f, 2f] conforme correto.png / QuestPDF
        };

        double headerHeight = 11.0;
        if (drawColumnHeaders)
        {
            DrawColumnHeaders(gfx, styles, solidPen, columns, colWidths, x, currentY, headerHeight);
            currentY += headerHeight;
        }

        double yInicio = currentY;

        int endIndex = slice.FirstItemIndex + slice.ItemCount;
        for (int i = slice.FirstItemIndex; i < endIndex && i < allItems.Count; i++)
        {
            double currentRowHeight = rowHeights[i];

            // 1. Desenhar texto dos produtos (com quebra automática na coluna de descrição)
            DrawItemRowText(gfx, styles, allItems[i], columns, colWidths, x, currentY, currentRowHeight);

            // 2. Divisórias verticais internas tracejadas
            double tempX = x;
            for (int c = 0; c < columns.Count - 1; c++)
            {
                tempX += colWidths[c];
                gfx.DrawLine(dashedPen, tempX, currentY, tempX, currentY + currentRowHeight);
            }

            // 3. Linha horizontal interna divisória tracejada
            gfx.DrawLine(dashedPen, x, currentY + currentRowHeight, x + availableWidth, currentY + currentRowHeight);

            currentY += currentRowHeight;
        }

        double yAtual = currentY;
        double minHeight = slice.AvailableHeight;
        double yFimAlvo = yInicio + minHeight;

        // Se a altura útil for maior que a desenhada, estende as divisórias internas como tracejado
        if (yAtual < yFimAlvo)
        {
            double tempX = x;
            for (int c = 0; c < columns.Count - 1; c++)
            {
                tempX += colWidths[c];
                gfx.DrawLine(dashedPen, tempX, yAtual, tempX, yFimAlvo); // Extensão vertical tracejada das colunas
            }
            currentY = yFimAlvo;
        }

        // Desenhar a moldura externa do bloco inteiramente SÓLIDA
        gfx.DrawLine(solidPen, x, y, x, yFimAlvo); // Lateral esquerda
        gfx.DrawLine(solidPen, x + availableWidth, y, x + availableWidth, yFimAlvo); // Lateral direita
        gfx.DrawLine(solidPen, x, yFimAlvo, x + availableWidth, yFimAlvo); // Base inferior

        return currentY;
    }

    private static void DrawColumnHeaders(
        XGraphics gfx, DanfeStyleCatalog styles, XPen solidPen,
        IReadOnlyList<DanfeItemColumn> columns,
        double[] colWidths,
        double x, double y, double rowHeight)
    {
        double currentX = x;
        var headerFont = new XFont(DanfeFontResolver.FamilyName, 4.2, XFontStyleEx.Bold);

        var tf = new XTextFormatter(gfx)
        {
            Alignment = XParagraphAlignment.Center
        };

        for (int i = 0; i < columns.Count; i++)
        {
            double colWidth = colWidths[i];
            var rect = new XRect(currentX, y, colWidth, rowHeight);
            
            // Desenhar célula sólida para o cabeçalho
            gfx.DrawRectangle(solidPen, rect);
            
            double padding = 1.0;
            var textRect = new XRect(currentX + padding, y + 1.0, colWidth - (padding * 2), rowHeight - 2.0);
            tf.DrawString(columns[i].Header, headerFont, styles.TextBrush, textRect);
            
            currentX += colWidth;
        }
    }

    private static void DrawItemRowText(
        XGraphics gfx, DanfeStyleCatalog styles,
        DanfeItemRow item, IReadOnlyList<DanfeItemColumn> columns,
        double[] colWidths,
        double x, double y, double rowHeight)
    {
        double currentX = x;
        string[] values = GetItemValues(item);
        var valueFont = new XFont(DanfeFontResolver.FamilyName, 5.0, XFontStyleEx.Regular);

        for (int c = 0; c < columns.Count && c < values.Length; c++)
        {
            double colWidth = colWidths[c];
            double padding = 1.0;
            double textMaxWidth = colWidth - (padding * 2);

            var textFormat = new XStringFormat
            {
                Alignment = columns[c].Alignment,
                LineAlignment = XLineAlignment.Center
            };

            var textRect = new XRect(currentX + padding, y, colWidth - (padding * 2), rowHeight);

            if (c == 1) // DESCRIÇÃO DO PRODUTO/SERVIÇO
            {
                var tf = new XTextFormatter(gfx);
                // Pequeno padding vertical de 1.0 pt para o texto não ficar colado nas linhas horizontais
                var descRect = new XRect(currentX + padding, y + 1.0, colWidth - (padding * 2), rowHeight - 2.0);
                tf.DrawString(values[c], valueFont, styles.TextBrush, descRect);
            }
            else
            {
                gfx.DrawString(values[c], valueFont, styles.TextBrush, textRect, textFormat);
            }

            currentX += colWidth;
        }
    }

    private static double[] GetColumnWidths(double totalWidth, IReadOnlyList<DanfeItemColumn> columns)
    {
        double constantSum = 0;
        int relativeCount = 0;
        foreach (var col in columns)
        {
            if (col.WidthPt > 0)
                constantSum += col.WidthPt;
            else
                relativeCount++;
        }

        double relativeWidth = 0;
        if (relativeCount > 0)
        {
            relativeWidth = (totalWidth - constantSum) / relativeCount;
            if (relativeWidth < 10) relativeWidth = 10;
        }

        var widths = new double[columns.Count];
        for (int i = 0; i < columns.Count; i++)
        {
            widths[i] = columns[i].WidthPt > 0 ? columns[i].WidthPt : relativeWidth;
        }
        return widths;
    }

    private static string[] GetItemValues(DanfeItemRow item)
    {
        return new[]
        {
            item.CodigoProduto,
            item.Descricao,
            item.Ncm,
            item.Cst,
            item.Cfop,
            item.Unidade,
            item.Quantidade,
            item.ValorUnitario,
            item.ValorTotal,
            item.ValorDesconto,
            item.BaseIcms,
            item.ValorIcms,
            item.ValorIpi,
            item.AliquotaIcms,
            item.AliquotaIpi
        };
    }
}
