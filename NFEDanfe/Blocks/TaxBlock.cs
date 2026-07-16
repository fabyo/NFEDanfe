using System;
using PdfSharp.Drawing;
using NFEDanfe.Builder;
using NFEDanfe.Domain.Models;
using NFEDanfe.Domain.Formatting;

namespace NFEDanfe.Blocks;

/// <summary>Bloco de cálculo do imposto / totais.</summary>
internal sealed class TaxBlock
{
    private static double Mm(double mm) => mm * 2.834645;

    private readonly DanfeModel _model;

    internal TaxBlock(DanfeModel model)
    {
        _model = model;
    }

    /// <summary>Desenha o bloco de impostos. Retorna a altura consumida.</summary>
    internal double Draw(XGraphics gfx, DanfeStyleCatalog styles, double x, double y, double width, bool isLandscape = false)
    {
        double titleH = 12.0;
        double fieldH = isLandscape ? 18.0 : DanfeField.DefaultHeight(styles);
        double height = titleH + fieldH * 2.0;
        var valueFontOverride = isLandscape ? new XFont(DanfeFontResolver.FamilyName, 7.0, XFontStyleEx.Regular) : null;

        // 1. Título "CÁLCULO DO IMPOSTO" com fundo cinza claro e borda
        var grayBrush = new XSolidBrush(XColor.FromArgb(224, 224, 224));
        gfx.DrawRectangle(styles.BorderPen, grayBrush, x, y, width, titleH);

        var titleFormat = new XStringFormat { Alignment = XStringAlignment.Near, LineAlignment = XLineAlignment.Center };
        var titleFont = new XFont(DanfeFontResolver.FamilyName, 6.0, XFontStyleEx.Bold);
        gfx.DrawString("CÁLCULO DO IMPOSTO", titleFont, styles.TextBrush,
            new XRect(x + 4, y, width - 8, titleH), titleFormat);
        y += titleH;

        var imp = _model.Impostos;
        double cellW = width / 9.0;

        // 2. Linha 1: 9 células de igual largura
        double currentX = x;
        DrawMoneyCell(gfx, styles, "BASE DE CÁLCULO DO ICMS", imp?.BaseCalculoIcms ?? 0, currentX, y, cellW, fieldH, valueFontOverride);
        currentX += cellW;
        DrawMoneyCell(gfx, styles, "VALOR DO ICMS", imp?.ValorIcms ?? 0, currentX, y, cellW, fieldH, valueFontOverride);
        currentX += cellW;
        DrawMoneyCell(gfx, styles, "BASE DE CÁLCULO DO ICMS ST", imp?.BaseCalculoIcmsSt ?? 0, currentX, y, cellW, fieldH, valueFontOverride);
        currentX += cellW;
        DrawMoneyCell(gfx, styles, "VALOR DO ICMS ST", imp?.ValorIcmsSt ?? 0, currentX, y, cellW, fieldH, valueFontOverride);
        currentX += cellW;
        DrawMoneyCell(gfx, styles, "FCP", imp?.ValorFcp ?? 0, currentX, y, cellW, fieldH, valueFontOverride);
        currentX += cellW;
        DrawMoneyCell(gfx, styles, "VALOR IMP. IMPORTAÇÃO", imp?.ValorIi ?? 0, currentX, y, cellW, fieldH, valueFontOverride);
        currentX += cellW;
        DrawMoneyCell(gfx, styles, "VALOR ICMS UF REMET.", imp?.ValorIcmsUfRemet ?? 0, currentX, y, cellW, fieldH, valueFontOverride);
        currentX += cellW;
        DrawMoneyCell(gfx, styles, "VALOR DO PIS", imp?.ValorPis ?? 0, currentX, y, cellW, fieldH, valueFontOverride);
        currentX += cellW;
        DrawMoneyCell(gfx, styles, "VALOR TOTAL DOS PRODUTOS", imp?.ValorProdutos ?? 0, currentX, y, cellW, fieldH, valueFontOverride);

        y += fieldH;

        // 3. Linha 2: 9 células de igual largura
        currentX = x;
        DrawMoneyCell(gfx, styles, "VALOR DO FRETE", imp?.ValorFrete ?? 0, currentX, y, cellW, fieldH, valueFontOverride);
        currentX += cellW;
        DrawMoneyCell(gfx, styles, "VALOR DO SEGURO", imp?.ValorSeguro ?? 0, currentX, y, cellW, fieldH, valueFontOverride);
        currentX += cellW;
        DrawMoneyCell(gfx, styles, "DESCONTO", imp?.ValorDesconto ?? 0, currentX, y, cellW, fieldH, valueFontOverride);
        currentX += cellW;
        DrawMoneyCell(gfx, styles, "OUTRAS DESPESAS ACESSÓRIAS", imp?.OutrasDespesas ?? 0, currentX, y, cellW, fieldH, valueFontOverride);
        currentX += cellW;
        DrawMoneyCell(gfx, styles, "VALOR TOTAL IPI", imp?.ValorIpi ?? 0, currentX, y, cellW, fieldH, valueFontOverride);
        currentX += cellW;
        DrawMoneyCell(gfx, styles, "VALOR ICMS UF DEST", imp?.ValorIcmsUfDest ?? 0, currentX, y, cellW, fieldH, valueFontOverride);
        currentX += cellW;
        DrawMoneyCell(gfx, styles, "VALOR TOTAL TRIB.", imp?.ValorTotTrib ?? 0, currentX, y, cellW, fieldH, valueFontOverride);
        currentX += cellW;
        DrawMoneyCell(gfx, styles, "VALOR DA COFINS", imp?.ValorCofins ?? 0, currentX, y, cellW, fieldH, valueFontOverride);
        currentX += cellW;
        DrawMoneyCell(gfx, styles, "VALOR TOTAL DA NOTA", imp?.ValorNota ?? 0, currentX, y, cellW, fieldH, valueFontOverride);

        return height;
    }

    private static void DrawMoneyCell(XGraphics gfx, DanfeStyleCatalog styles, string label, decimal value, double cx, double cy, double cw, double ch, XFont? valueFontOverride = null)
    {
        string valStr = DocumentFormatter.Money(value);

        // Criar uma fonte menor de 4.3 pt para os labels do cálculo do imposto não cortarem
        var labelFont = new XFont(DanfeFontResolver.FamilyName, 4.3, XFontStyleEx.Regular);

        // Renderizar a célula usando DanfeField (com alinhamento à direita (Far) dos valores)
        var field = new DanfeField(label, valStr, 0, XStringAlignment.Far);
        field.Draw(gfx, styles, cx, cy, cw, ch, labelFont, valueFontOverride);
    }
}
