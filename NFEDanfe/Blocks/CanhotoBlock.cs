using System;
using System.Collections.Generic;
using System.Linq;
using PdfSharp.Drawing;
using NFEDanfe.Domain.Models;
using NFEDanfe.Domain.Formatting;

namespace NFEDanfe.Blocks;

/// <summary>Bloco do canhoto do DANFE.</summary>
internal sealed class CanhotoBlock
{
    private static double Mm(double mm) => mm * 2.834645;

    private readonly DanfeModel _model;

    internal CanhotoBlock(DanfeModel model)
    {
        _model = model;
    }

    private sealed class TextRun
    {
        public string Text { get; }
        public bool IsBold { get; }
        public TextRun(string text, bool isBold)
        {
            Text = text;
            IsBold = isBold;
        }
    }

    /// <summary>Desenha o canhoto no modo retrato. Retorna a altura total consumida.</summary>
    internal double Draw(XGraphics gfx, DanfeStyleCatalog styles, double x, double y, double width)
    {
        double topTextHeight = 44.0;
        double fieldsHeight = 24.0;
        double height = topTextHeight + fieldsHeight; // 68.0 pt

        // Left box (9.5/12 of width) and Right box (2.5/12 of width)
        double leftWidth = width * 9.5 / 12.0;
        double rightWidth = width * 2.5 / 12.0;

        // Borda externa da parte esquerda
        gfx.DrawRectangle(styles.BorderPen, x, y, leftWidth, height);
        // Borda externa da parte direita
        gfx.DrawRectangle(styles.BorderPen, x + leftWidth, y, rightWidth, height);

        // --- LADO ESQUERDO ---
        // 1. Texto de recebimento rico com partes em negrito
        var runs = new List<TextRun>
        {
            new TextRun("RECEBEMOS DE ", false),
            new TextRun(_model.Emitente.RazaoSocial.ToUpper(), true),
            new TextRun(" OS PRODUTOS E/OU SERVIÇOS CONSTANTES DA NOTA FISCAL ELETRÔNICA INDICADA ABAIXO. EMISSÃO: ", false),
            new TextRun(_model.DadosDanfe.DataEmissao.ToString("dd/MM/yyyy"), true),
            new TextRun(" - VALOR TOTAL: R$ ", false),
            new TextRun(DocumentFormatter.Money(_model.ValorTotal), true),
            new TextRun(" - DESTINATÁRIO: ", false),
            new TextRun(_model.Destinatario.RazaoSocial.ToUpper(), true),
            new TextRun(" - " + DocumentFormatter.FullAddress(_model.Destinatario.Endereco).ToUpper(), false)
        };

        var textRect = new XRect(x + 4, y + 4, leftWidth - 8, topTextHeight - 4);
        DrawRichText(gfx, styles, textRect, runs);

        // 2. Linha separadora horizontal entre texto e campos
        double fieldsY = y + topTextHeight;
        gfx.DrawLine(styles.BorderPen, x, fieldsY, x + leftWidth, fieldsY);

        // 3. Campos de Data de Recebimento e Assinatura
        double dateWidth = leftWidth * 3.0 / 12.0;
        double signatureWidth = leftWidth * 9.0 / 12.0;

        // Separador vertical dos campos
        gfx.DrawLine(styles.BorderPen, x + dateWidth, fieldsY, x + dateWidth, fieldsY + fieldsHeight);

        // Label "DATA DE RECEBIMENTO"
        var labelNear = new XStringFormat { Alignment = XStringAlignment.Near, LineAlignment = XLineAlignment.Near };
        gfx.DrawString("DATA DE RECEBIMENTO", styles.LabelFont, styles.TextBrush,
            new XRect(x + 3, fieldsY + 3, dateWidth - 6, fieldsHeight - 6), labelNear);

        // Label "IDENTIFICAÇÃO E ASSINATURA DO RECEBEDOR"
        gfx.DrawString("IDENTIFICAÇÃO E ASSINATURA DO RECEBEDOR", styles.LabelFont, styles.TextBrush,
            new XRect(x + dateWidth + 3, fieldsY + 3, signatureWidth - 6, fieldsHeight - 6), labelNear);


        // --- LADO DIREITO (NF-e info) ---
        var centerFormat = new XStringFormat
        {
            Alignment = XStringAlignment.Center,
            LineAlignment = XLineAlignment.Center
        };

        double rightX = x + leftWidth;
        double blockH = height / 3.0;

        var titleFont = new XFont(DanfeFontResolver.FamilyName, 11, XFontStyleEx.Bold);
        gfx.DrawString("NF-e", titleFont, styles.TextBrush,
            new XRect(rightX, y + 2, rightWidth, blockH), centerFormat);

        gfx.DrawString($"Nº {_model.DadosDanfe.Numero:N0}", styles.ValueBoldFont, styles.TextBrush,
            new XRect(rightX, y + blockH, rightWidth, blockH), centerFormat);

        gfx.DrawString($"Série {_model.DadosDanfe.Serie:000}", styles.ValueBoldFont, styles.TextBrush,
            new XRect(rightX, y + blockH * 2 - 2, rightWidth, blockH), centerFormat);


        // Separador pontilhado abaixo do canhoto
        double separatorY = y + height + 3;
        var dashPen = new XPen(XColors.Black, 0.75) { DashPattern = new[] { 4.0, 4.0 } };
        gfx.DrawLine(dashPen, x, separatorY, x + width, separatorY);

        return height + 6;
    }

    private static void DrawRichText(XGraphics gfx, DanfeStyleCatalog styles, XRect rect, List<TextRun> runs)
    {
        var regularFont = styles.LabelFont;
        var boldFont = new XFont(DanfeFontResolver.FamilyName, regularFont.Size, XFontStyleEx.Bold);

        double lineSpacing = regularFont.Size + 1.2; // Espaçamento entre linhas confortável
        double currentX = rect.X;
        double currentY = rect.Y + regularFont.Size;

        var formattedWords = new List<(string text, XFont font, bool isSpace)>();

        foreach (var run in runs)
        {
            var font = run.IsBold ? boldFont : regularFont;
            string text = run.Text;
            int idx = 0;
            while (idx < text.Length)
            {
                if (text[idx] == ' ')
                {
                    formattedWords.Add((" ", font, true));
                    idx++;
                }
                else
                {
                    int start = idx;
                    while (idx < text.Length && text[idx] != ' ')
                    {
                        idx++;
                    }
                    formattedWords.Add((text[start..idx], font, false));
                }
            }
        }

        foreach (var item in formattedWords)
        {
            if (item.isSpace)
            {
                double spaceWidth = gfx.MeasureString(" ", item.font).Width;
                if (currentX > rect.X)
                {
                    currentX += spaceWidth;
                }
            }
            else
            {
                double wordWidth = gfx.MeasureString(item.text, item.font).Width;

                if (currentX + wordWidth > rect.Right && currentX > rect.X)
                {
                    currentX = rect.X;
                    currentY += lineSpacing;
                }

                gfx.DrawString(item.text, item.font, styles.TextBrush, currentX, currentY);
                if (item.font.Style == XFontStyleEx.Bold)
                {
                    gfx.DrawString(item.text, item.font, styles.TextBrush, currentX + 0.35, currentY);
                }
                currentX += wordWidth;
            }
        }
    }

    /// <summary>Desenha o canhoto no modo paisagem (rotacionado −90° no canto esquerdo).</summary>
    internal double DrawLandscape(XGraphics gfx, DanfeStyleCatalog styles, double x, double y, double usableHeight)
    {
        double stripWidth = Mm(28);
        var state = gfx.Save();

        gfx.TranslateTransform(x, y + usableHeight);
        gfx.RotateTransform(-90);

        Draw(gfx, styles, 0, 0, usableHeight);

        gfx.Restore(state);
        return stripWidth;
    }
}
