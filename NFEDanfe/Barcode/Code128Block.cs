using System;
using System.Collections.Generic;
using PdfSharp.Drawing;

namespace NFEDanfe.Barcode;

/// <summary>Componente que injeta o Code 128 no rodapé fiscal.</summary>
internal sealed class Code128Block
{
    private readonly string _chaveAcesso;

    /// <summary>Cria o bloco de Code 128 com a chave de acesso.</summary>
    internal Code128Block(string chaveAcesso)
    {
        if (string.IsNullOrEmpty(chaveAcesso) || chaveAcesso.Length != 44)
            throw new ArgumentException(
                $"A chave de acesso deve conter exatamente 44 dígitos numéricos. Recebido: '{chaveAcesso}' ({chaveAcesso?.Length ?? 0} caracteres).",
                nameof(chaveAcesso));

        _chaveAcesso = chaveAcesso;
    }

    /// <summary>Desenha o Code 128 na área reservada.</summary>
    internal void Draw(XGraphics gfx, DanfeStyleCatalog styles,
                       double x, double y, double width, double height)
    {
        double textReserve = 4.0;
        double barHeight = height - textReserve;

        // Renderizar barras
        BarcodeRenderer.DrawCode128(gfx, _chaveAcesso, x, y, width, barHeight, styles.TextBrush);

        // Texto da chave formatada abaixo das barras
        string chaveFormatada = FormatChave(_chaveAcesso);
        var format = new XStringFormat
        {
            Alignment = XStringAlignment.Center,
            LineAlignment = XLineAlignment.Near
        };

        gfx.DrawString(chaveFormatada, styles.LabelFont, styles.TextBrush,
            new XRect(x, y + barHeight + 1, width, textReserve), format);
    }

    private static string FormatChave(string chave)
    {
        var parts = new List<string>();
        for (int i = 0; i < 44; i += 4)
        {
            int len = Math.Min(4, 44 - i);
            parts.Add(chave.Substring(i, len));
        }
        return string.Join(" ", parts);
    }
}
