using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using NFEDanfe.Domain.Models;

namespace NFEDanfe.Blocks;

/// <summary>Bloco de dados adicionais e reservado ao fisco.</summary>
internal sealed class AdditionalDataBlock
{
    private static double Mm(double mm) => mm * 2.834645;

    private readonly DanfeModel _model;

    internal AdditionalDataBlock(DanfeModel model)
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

    /// <summary>Desenha o bloco de dados adicionais. Retorna a altura consumida.</summary>
    internal double Draw(XGraphics gfx, DanfeStyleCatalog styles, double x, double y, double width)
    {
        double titleH = 12.0;
        double contentMinH = 70.0;
        double height = titleH + contentMinH;

        // 1. Título "DADOS ADICIONAIS" com fundo cinza claro e borda
        var grayBrush = new XSolidBrush(XColor.FromArgb(224, 224, 224));
        gfx.DrawRectangle(styles.BorderPen, grayBrush, x, y, width, titleH);

        var titleFormat = new XStringFormat { Alignment = XStringAlignment.Near, LineAlignment = XLineAlignment.Center };
        var titleFont = new XFont(DanfeFontResolver.FamilyName, 6.0, XFontStyleEx.Bold);
        gfx.DrawString("DADOS ADICIONAIS", titleFont, styles.TextBrush,
            new XRect(x + 4, y, width - 8, titleH), titleFormat);
        y += titleH;

        // 2. Colunas de Informações Complementares (8/12) e Reservado ao Fisco (4/12)
        double leftWidth = width * 8.0 / 12.0;
        double rightWidth = width * 4.0 / 12.0;

        // Borda externa
        gfx.DrawRectangle(styles.BorderPen, x, y, width, contentMinH);
        // Separador vertical
        gfx.DrawLine(styles.BorderPen, x + leftWidth, y, x + leftWidth, y + contentMinH);

        var labelNear = new XStringFormat { Alignment = XStringAlignment.Near, LineAlignment = XLineAlignment.Near };
        var labelFont = new XFont(DanfeFontResolver.FamilyName, 5.0, XFontStyleEx.Bold);

        // --- LADO ESQUERDO: INFORMAÇÕES COMPLEMENTARES ---
        gfx.DrawString("INFORMAÇÕES COMPLEMENTARES", labelFont, styles.TextBrush,
            new XRect(x + 4, y + 4, leftWidth - 8, 8), labelNear);

        // Processamento do texto (separado por ';' e adicionando Pedido)
        var infComplRaw = _model.DadosAdicionais?.InformacoesComplementares ?? string.Empty;
        var infComplLines = infComplRaw.Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrEmpty(l))
            .ToList();

        string pedestrianText = string.Join("\n", infComplLines);
        string pedido = string.Join(", ", _model.DadosAdicionais?.PedidosCompra ?? Array.Empty<string>());
        if (!string.IsNullOrWhiteSpace(pedido))
        {
            if (infComplLines.Count > 0)
            {
                pedestrianText += "\n";
            }
            pedestrianText += $"Pedido: {pedido}";
        }

        if (!string.IsNullOrEmpty(pedestrianText))
        {
            var textRect = new XRect(x + 4, y + 14, leftWidth - 8, contentMinH - 18);
            
            // Construir runs formatados para e-mail e pedido em negrito
            var runs = new List<TextRun>();
            var lines = pedestrianText.Split('\n');
            
            for (int l = 0; l < lines.Length; l++)
            {
                var line = lines[l];
                string[] words = line.Split(' ');
                bool boldNext = false;
                
                for (int i = 0; i < words.Length; i++)
                {
                    string word = words[i];
                    bool isBold = false;
                    
                    string cleanDigits = new string(word.Where(char.IsDigit).ToArray());

                    if (word.Contains("@") && word.Contains("."))
                    {
                        isBold = true;
                    }
                    else if (cleanDigits.Length == 44)
                    {
                        isBold = true;
                    }
                    else if (Regex.IsMatch(word, @"\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}"))
                    {
                        isBold = true;
                    }
                    else if (boldNext)
                    {
                        isBold = true;
                    }
                    else if (string.Equals(word, "Pedido:", StringComparison.OrdinalIgnoreCase))
                    {
                        isBold = true;
                        boldNext = true;
                    }
                    
                    runs.Add(new TextRun(word, isBold));
                    if (i < words.Length - 1)
                    {
                        runs.Add(new TextRun(" ", false));
                    }
                }
                
                if (l < lines.Length - 1)
                {
                    runs.Add(new TextRun("\n", false));
                }
            }

            DrawRichText(gfx, styles, textRect, runs, 6.0);
        }

        // --- LADO DIREITO: RESERVADO AO FISCO ---
        gfx.DrawString("RESERVADO AO FISCO", labelFont, styles.TextBrush,
            new XRect(x + leftWidth + 4, y + 4, rightWidth - 8, 8), labelNear);

        var infFiscoRaw = _model.DadosAdicionais?.InformacoesFisco ?? string.Empty;
        var infFiscoLines = infFiscoRaw.Split(';', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrEmpty(l));
        string infFiscoFormatted = string.Join("\n", infFiscoLines);

        if (!string.IsNullOrEmpty(infFiscoFormatted))
        {
            var tf = new XTextFormatter(gfx);
            var textRect = new XRect(x + leftWidth + 4, y + 14, rightWidth - 8, contentMinH - 18);
            tf.DrawString(infFiscoFormatted, new XFont(DanfeFontResolver.FamilyName, 6.0, XFontStyleEx.Regular), styles.TextBrush, textRect);
        }

        return height;
    }

    private static void DrawRichText(XGraphics gfx, DanfeStyleCatalog styles, XRect rect, List<TextRun> runs, double fontSize)
    {
        var regularFont = new XFont(DanfeFontResolver.FamilyName, fontSize, XFontStyleEx.Regular);
        var boldFont = new XFont(DanfeFontResolver.FamilyName, fontSize, XFontStyleEx.Bold);
        
        double lineSpacing = fontSize + 1.2;
        double currentX = rect.X;
        double currentY = rect.Y + fontSize;

        var formattedWords = new List<(string text, XFont font, bool isSpace, bool isNewline)>();

        foreach (var run in runs)
        {
            var font = run.IsBold ? boldFont : regularFont;
            string text = run.Text;
            
            if (text == "\n")
            {
                formattedWords.Add(("\n", font, false, true));
                continue;
            }

            int idx = 0;
            while (idx < text.Length)
            {
                if (text[idx] == ' ')
                {
                    formattedWords.Add((" ", font, true, false));
                    idx++;
                }
                else
                {
                    int start = idx;
                    while (idx < text.Length && text[idx] != ' ')
                    {
                        idx++;
                    }
                    formattedWords.Add((text[start..idx], font, false, false));
                }
            }
        }

        foreach (var item in formattedWords)
        {
            if (item.isNewline)
            {
                currentX = rect.X;
                currentY += lineSpacing;
            }
            else if (item.isSpace)
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

                if (item.font.Style == XFontStyleEx.Bold)
                {
                    gfx.DrawString(item.text, item.font, styles.TextBrush, currentX, currentY);
                    gfx.DrawString(item.text, item.font, styles.TextBrush, currentX + 0.35, currentY);
                }
                else
                {
                    gfx.DrawString(item.text, item.font, styles.TextBrush, currentX, currentY);
                }
                currentX += wordWidth;
            }
        }
    }
}
