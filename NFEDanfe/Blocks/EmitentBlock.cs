using System;
using System.IO;
using PdfSharp.Drawing;
using NFEDanfe.Domain.Models;
using NFEDanfe.Domain.Formatting;
using NFEDanfe.Barcode;

namespace NFEDanfe.Blocks;

/// <summary>Bloco do emitente que engloba EmitenteBox, DanfeBox e ChaveAcessoBox lado a lado.</summary>
internal sealed class EmitentBlock
{
    private static double Mm(double mm) => mm * 2.834645;

    private readonly DanfeModel _model;
    private readonly string? _logoPath;
    private readonly byte[]? _logoBytes;
    private readonly int _currentPage;
    private readonly int _totalPages;

    internal EmitentBlock(DanfeModel model, string? logoPath, byte[]? logoBytes, int currentPage, int totalPages)
    {
        _model = model;
        _logoPath = logoPath;
        _logoBytes = logoBytes;
        _currentPage = currentPage;
        _totalPages = totalPages;
    }

    /// <summary>Desenha as três caixas lado a lado. Retorna a altura consumida (40mm).</summary>
    internal double Draw(XGraphics gfx, DanfeStyleCatalog styles, double x, double y, double width, bool isLandscape = false)
    {
        double height = isLandscape ? 50.0 : 90.0; // Altura compactada em paisagem (reduzida mais 1 linha, total 50pt)

        double colEmitenteW = width * 5.0 / 12.0;
        double colDanfeW = width * 2.0 / 12.0;
        double colChaveW = width * 5.0 / 12.0;

        // Desenhar os 3 retângulos de borda
        gfx.DrawRectangle(styles.BorderPen, x, y, colEmitenteW, height);
        gfx.DrawRectangle(styles.BorderPen, x + colEmitenteW, y, colDanfeW, height);
        gfx.DrawRectangle(styles.BorderPen, x + colEmitenteW + colDanfeW, y, colChaveW, height);

        // ==========================================
        // 1. EMITENTE BOX (Lado Esquerdo)
        // ==========================================
        double emitenteContentX = x + 4;
        double emitenteContentW = colEmitenteW - 8;
        double logoSpace = isLandscape ? 60.0 : 70.0;

        // Se houver logotipo, desenha reservando espaço correspondente
        if (_logoBytes != null && _logoBytes.Length > 0)
        {
            try
            {
                using var ms = new MemoryStream();
                ms.Write(_logoBytes, 0, _logoBytes.Length);
                ms.Position = 0;
                using var image = XImage.FromStream(ms);
                double imgW = isLandscape ? 55.0 : 65.0;
                double imgH = isLandscape ? 34.0 : 55.0;
                double scale = Math.Min(imgW / image.PixelWidth, imgH / image.PixelHeight);
                double finalW = image.PixelWidth * scale;
                double finalH = image.PixelHeight * scale;
                gfx.DrawImage(image, emitenteContentX, y + (height - finalH) / 2, finalW, finalH);
                
                emitenteContentX += logoSpace;
                emitenteContentW -= logoSpace;
            }
            catch { }
        }
        else if (!string.IsNullOrEmpty(_logoPath) && File.Exists(_logoPath))
        {
            try
            {
                using var image = XImage.FromFile(_logoPath);
                double imgW = isLandscape ? 55.0 : 65.0;
                double imgH = isLandscape ? 34.0 : 55.0;
                double scale = Math.Min(imgW / image.PixelWidth, imgH / image.PixelHeight);
                double finalW = image.PixelWidth * scale;
                double finalH = image.PixelHeight * scale;
                gfx.DrawImage(image, emitenteContentX, y + (height - finalH) / 2, finalW, finalH);
                
                emitenteContentX += logoSpace;
                emitenteContentW -= logoSpace;
            }
            catch { }
        }

        // Razão Social do Emitente (Destaque)
        var formatEmit = new XStringFormat { Alignment = XStringAlignment.Near, LineAlignment = XLineAlignment.Near };
        double emitY = y + (isLandscape ? 2.0 : 5.0);
        var rsFont = new XFont(DanfeFontResolver.FamilyName, isLandscape ? 7.0 : 9.0, XFontStyleEx.Bold);
        double rsLineSpacing = (isLandscape ? 7.0 : 9.0) + 1.2;
        var rsRect = new XRect(emitenteContentX, emitY, emitenteContentW, isLandscape ? 20.0 : 25.0);
        double consumedH = DrawWrappedString(gfx, _model.Emitente.RazaoSocial.ToUpper(), rsFont, styles.TextBrush, rsRect, rsLineSpacing, isBold: true);
        emitY += consumedH + 2.0;

        // Nome Fantasia (Itálico se existir e for diferente)
        if (!string.IsNullOrEmpty(_model.Emitente.NomeFantasia) && 
            !_model.Emitente.NomeFantasia.Equals(_model.Emitente.RazaoSocial, StringComparison.OrdinalIgnoreCase))
        {
            var fantFont = new XFont(DanfeFontResolver.FamilyName, isLandscape ? 6.0 : 8.0, XFontStyleEx.Italic);
            gfx.DrawString(_model.Emitente.NomeFantasia.ToUpper(), fantFont, styles.TextBrush,
                new XRect(emitenteContentX, emitY, emitenteContentW, isLandscape ? 7.0 : 10.0), formatEmit);
            emitY += isLandscape ? 7.0 : 10.0;
        }

        // Endereço e Detalhes
        string address1 = $"{_model.Emitente.Endereco.Logradouro}, {_model.Emitente.Endereco.Numero}";
        if (!string.IsNullOrEmpty(_model.Emitente.Endereco.Complemento))
        {
            address1 += $" - {_model.Emitente.Endereco.Complemento}";
        }
        string address2 = $"{_model.Emitente.Endereco.Bairro} - CEP: {DocumentFormatter.Cep(_model.Emitente.Endereco.Cep)}";
        string address3 = $"{_model.Emitente.Endereco.Municipio} - {_model.Emitente.Endereco.Uf}";
        if (!string.IsNullOrEmpty(_model.Emitente.Telefone))
        {
            address3 += $" - Fone: {_model.Emitente.Telefone}";
        }

        var detailFont = new XFont(DanfeFontResolver.FamilyName, isLandscape ? 5.2 : 6.0, XFontStyleEx.Regular);
        double addressLineH = isLandscape ? 5.5 : 8.0;
        gfx.DrawString(address1, detailFont, styles.TextBrush, new XRect(emitenteContentX, emitY, emitenteContentW, addressLineH), formatEmit);
        emitY += addressLineH;
        gfx.DrawString(address2, detailFont, styles.TextBrush, new XRect(emitenteContentX, emitY, emitenteContentW, addressLineH), formatEmit);
        emitY += addressLineH;
        gfx.DrawString(address3, detailFont, styles.TextBrush, new XRect(emitenteContentX, emitY, emitenteContentW, addressLineH), formatEmit);


        // ==========================================
        // 2. DANFE BOX (Centro)
        // ==========================================
        double danfeX = x + colEmitenteW;
        double danfeY = y + (isLandscape ? 1.0 : 3.0);
        var centerFormat = new XStringFormat { Alignment = XStringAlignment.Center, LineAlignment = XLineAlignment.Near };

        // "DANFE"
        var danfeTitleFont = new XFont(DanfeFontResolver.FamilyName, isLandscape ? 8.0 : 11.0, XFontStyleEx.Bold);
        gfx.DrawString("DANFE", danfeTitleFont, styles.TextBrush, new XRect(danfeX, danfeY, colDanfeW, isLandscape ? 9.0 : 14.0), centerFormat);
        gfx.DrawString("DANFE", danfeTitleFont, styles.TextBrush, new XRect(danfeX + 0.35, danfeY, colDanfeW, isLandscape ? 9.0 : 14.0), centerFormat);
        danfeY += isLandscape ? 9.0 : 14.0;

        // Subtítulo
        var danfeSubFont = new XFont(DanfeFontResolver.FamilyName, isLandscape ? 4.8 : 6.0, XFontStyleEx.Regular);
        gfx.DrawString("Documento Auxiliar da", danfeSubFont, styles.TextBrush,
            new XRect(danfeX, danfeY, colDanfeW, isLandscape ? 5.5 : 8.0), centerFormat);
        gfx.DrawString("Nota Fiscal Eletrônica", danfeSubFont, styles.TextBrush,
            new XRect(danfeX, danfeY + (isLandscape ? 5.0 : 7.0), colDanfeW, isLandscape ? 5.5 : 8.0), centerFormat);
        danfeY += isLandscape ? 10.0 : 18.0;

        // 0 - Entrada / 1 - Saída Checkbox
        double checkX = danfeX + 4;
        double checkW = colDanfeW - 8;
        double checkRowH = isLandscape ? 9.0 : 15.0;

        var fontCheckLabel = new XFont(DanfeFontResolver.FamilyName, isLandscape ? 4.2 : 5.0, XFontStyleEx.Regular);
        var nearCenterFormat = new XStringFormat { Alignment = XStringAlignment.Near, LineAlignment = XLineAlignment.Center };
        gfx.DrawString("0 - Entrada", fontCheckLabel, styles.TextBrush,
            new XRect(checkX, danfeY, checkW - 16, checkRowH), nearCenterFormat);
        gfx.DrawString("1 - Saída", fontCheckLabel, styles.TextBrush,
            new XRect(checkX, danfeY + 7.5, checkW - 16, checkRowH), nearCenterFormat);

        // Caixa pequena do checkbox
        double boxSize = isLandscape ? 7.0 : 12.0;
        double boxX = checkX + checkW - boxSize - 2;
        double boxY = danfeY + (checkRowH - boxSize) / 2;
        gfx.DrawRectangle(styles.BorderPen, boxX, boxY, boxSize, boxSize);
        
        var checkFont = new XFont(DanfeFontResolver.FamilyName, isLandscape ? 7.0 : 9.0, XFontStyleEx.Bold);
        gfx.DrawString(_model.DadosDanfe.TipoOperacao.ToString(), checkFont, styles.TextBrush,
            new XRect(boxX, boxY, boxSize, boxSize), centerFormat);
        gfx.DrawString(_model.DadosDanfe.TipoOperacao.ToString(), checkFont, styles.TextBrush,
            new XRect(boxX + 0.35, boxY, boxSize, boxSize), centerFormat);

        danfeY += checkRowH + (isLandscape ? 1.0 : 4.0);

        // Número, Série e Folha
        if (isLandscape)
        {
            var numberFont = new XFont(DanfeFontResolver.FamilyName, 6.5, XFontStyleEx.Bold);
            gfx.DrawString($"Nº {_model.DadosDanfe.Numero:N0}", numberFont, styles.TextBrush,
                new XRect(danfeX, y + 30.0, colDanfeW, 8.0), centerFormat);
            gfx.DrawString($"Nº {_model.DadosDanfe.Numero:N0}", numberFont, styles.TextBrush,
                new XRect(danfeX + 0.35, y + 30.0, colDanfeW, 8.0), centerFormat);

            var serieFolhaFont = new XFont(DanfeFontResolver.FamilyName, 5.5, XFontStyleEx.Bold);
            gfx.DrawString($"SÉRIE {_model.DadosDanfe.Serie:000} - FOLHA {_currentPage}/{_totalPages}", serieFolhaFont, styles.TextBrush,
                new XRect(danfeX, y + 37.0, colDanfeW, 8.0), centerFormat);
            gfx.DrawString($"SÉRIE {_model.DadosDanfe.Serie:000} - FOLHA {_currentPage}/{_totalPages}", serieFolhaFont, styles.TextBrush,
                new XRect(danfeX + 0.35, y + 37.0, colDanfeW, 8.0), centerFormat);
        }
        else
        {
            var numberFont = new XFont(DanfeFontResolver.FamilyName, 8, XFontStyleEx.Bold);
            gfx.DrawString($"Nº {_model.DadosDanfe.Numero:N0}", numberFont, styles.TextBrush,
                new XRect(danfeX, danfeY, colDanfeW, 10), centerFormat);
            gfx.DrawString($"Nº {_model.DadosDanfe.Numero:N0}", numberFont, styles.TextBrush,
                new XRect(danfeX + 0.35, danfeY, colDanfeW, 10), centerFormat);
            danfeY += 10;

            gfx.DrawString($"SÉRIE {_model.DadosDanfe.Serie:000}", numberFont, styles.TextBrush,
                new XRect(danfeX, danfeY, colDanfeW, 10), centerFormat);
            gfx.DrawString($"SÉRIE {_model.DadosDanfe.Serie:000}", numberFont, styles.TextBrush,
                new XRect(danfeX + 0.35, danfeY, colDanfeW, 10), centerFormat);
            danfeY += 10;

            var folhaFont = new XFont(DanfeFontResolver.FamilyName, 8, XFontStyleEx.Regular);
            gfx.DrawString($"FOLHA: {_currentPage} / {_totalPages}", folhaFont, styles.TextBrush,
                new XRect(danfeX, danfeY, colDanfeW, 10), centerFormat);
        }


        // ==========================================
        // 3. CHAVE DE ACESSO BOX (Lado Direito)
        // ==========================================
        double chaveX = x + colEmitenteW + colDanfeW;

        // Barcode Code 128 (altura 15pt em paisagem, 28pt em retrato)
        double barcodePadding = 14.0;
        double barcodeW = colChaveW - (barcodePadding * 2);
        double barcodeH = isLandscape ? 15.0 : 28.0;
        double barcodeX = chaveX + barcodePadding;
        double barcodeY = y + 4;

        if (!string.IsNullOrEmpty(_model.DadosDanfe.ChaveAcesso))
        {
            try
            {
                BarcodeRenderer.DrawCode128(gfx, _model.DadosDanfe.ChaveAcesso, barcodeX, barcodeY, barcodeW, barcodeH, styles.TextBrush);
            }
            catch
            {
                gfx.DrawString("[ERRO AO GERAR CÓDIGO BARRAS]", styles.LabelFont, styles.TextBrush,
                    new XRect(barcodeX, barcodeY, barcodeW, barcodeH), centerFormat);
            }
        }

        // Label "CHAVE DE ACESSO"
        double labelY = barcodeY + barcodeH + (isLandscape ? 1.0 : 4.0);
        var formatLeft = new XStringFormat { Alignment = XStringAlignment.Near, LineAlignment = XLineAlignment.Near };
        gfx.DrawString("CHAVE DE ACESSO", styles.LabelFont, styles.TextBrush,
            new XRect(chaveX + 4, labelY, colChaveW - 8, 8), formatLeft);

        // Chave de acesso formatada
        double valY = labelY + (isLandscape ? 4.0 : 8.0);
        string formattedChave = FormatChave(_model.DadosDanfe.ChaveAcesso);
        var valFont = new XFont(DanfeFontResolver.FamilyName, isLandscape ? 5.5 : 7.0, XFontStyleEx.Bold);
        gfx.DrawString(formattedChave, valFont, styles.TextBrush,
            new XRect(chaveX + 4, valY, colChaveW - 8, 10), centerFormat);
        gfx.DrawString(formattedChave, valFont, styles.TextBrush,
            new XRect(chaveX + 4.35, valY, colChaveW - 8, 10), centerFormat);

        // Separador horizontal fino
        double lineY = valY + (isLandscape ? 7.0 : 12.0);
        gfx.DrawLine(styles.BorderPen, chaveX, lineY, chaveX + colChaveW, lineY);

        // Consulta de autenticidade texto
        double textY = lineY + (isLandscape ? 1.5 : 5.0);
        var infoFont = new XFont(DanfeFontResolver.FamilyName, isLandscape ? 4.5 : 5.5, XFontStyleEx.Regular);
        gfx.DrawString("Consulta de autenticidade no portal nacional da NF-e", infoFont, styles.TextBrush,
            new XRect(chaveX + 4, textY, colChaveW - 8, 8), centerFormat);
        gfx.DrawString("www.nfe.fazenda.gov.br/portal ou no site da Sefaz Autorizadora", infoFont, styles.TextBrush,
            new XRect(chaveX + 4, textY + (isLandscape ? 4.5 : 7.0), colChaveW - 8, 8), centerFormat);
        return height;
    }

    private static string FormatChave(string chave)
    {
        if (string.IsNullOrEmpty(chave) || chave.Length != 44) return chave;
        var parts = new System.Collections.Generic.List<string>();
        for (int i = 0; i < 44; i += 4)
        {
            int len = Math.Min(4, 44 - i);
            parts.Add(chave.Substring(i, len));
        }
        return string.Join(" ", parts);
    }

    private static double DrawWrappedString(XGraphics gfx, string text, XFont font, XSolidBrush brush, XRect rect, double lineSpacing, bool isBold = false)
    {
        string[] words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        double currentY = rect.Y;

        var lines = new List<List<string>>();
        var currentLine = new List<string>();
        double currentLineWidth = 0;

        foreach (var word in words)
        {
            double wordWidth = gfx.MeasureString(word, font).Width;
            double spaceWidth = gfx.MeasureString(" ", font).Width;

            if (currentLine.Count > 0)
            {
                if (currentLineWidth + spaceWidth + wordWidth > rect.Width)
                {
                    lines.Add(currentLine);
                    currentLine = new List<string> { word };
                    currentLineWidth = wordWidth;
                }
                else
                {
                    currentLine.Add(word);
                    currentLineWidth += spaceWidth + wordWidth;
                }
            }
            else
            {
                currentLine.Add(word);
                currentLineWidth = wordWidth;
            }
        }
        if (currentLine.Count > 0)
        {
            lines.Add(currentLine);
        }

        double yOffset = rect.Y;
        foreach (var line in lines)
        {
            string lineText = string.Join(" ", line);
            gfx.DrawString(lineText, font, brush, rect.X, yOffset + font.Size);
            if (isBold)
            {
                gfx.DrawString(lineText, font, brush, rect.X + 0.35, yOffset + font.Size);
            }
            yOffset += lineSpacing;
        }

        return yOffset - rect.Y;
    }
}
