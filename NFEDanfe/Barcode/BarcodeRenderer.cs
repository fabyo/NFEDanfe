using System;
using Barcoder;
using Barcoder.Code128;
using Barcoder.Qr;
using PdfSharp.Drawing;

namespace NFEDanfe.Barcode;

/// <summary>Renderizador vetorial de códigos de barras via PDFsharp.</summary>
internal static class BarcodeRenderer
{
    /// <summary>Renderiza Code 128 como retângulos vetoriais.</summary>
    internal static void DrawCode128(
        XGraphics gfx,
        string data,
        double x, double y,
        double availableWidth,
        double barHeight,
        XBrush darkBrush)
    {
        ArgumentNullException.ThrowIfNull(gfx);
        ArgumentNullException.ThrowIfNull(data);

        var barcode = Code128Encoder.Encode(data);
        int totalModules = barcode.Bounds.X;

        // Quiet zone of 10 modules on each side
        // moduleWidth = availableWidth / (totalModules + 20)
        double moduleWidth = availableWidth / (totalModules + 20);
        if (moduleWidth > 1.0) moduleWidth = 1.0;

        double totalBarcodeWidth = totalModules * moduleWidth;
        double offsetX = x + (availableWidth - totalBarcodeWidth) / 2;

        for (int c = 0; c < totalModules; c++)
        {
            for (int r = 0; r < barcode.Bounds.Y; r++)
            {
                if (barcode.At(c, r))
                {
                    gfx.DrawRectangle(darkBrush,
                        offsetX + c * moduleWidth,
                        y,
                        moduleWidth,
                        barHeight);
                }
            }
        }
    }

    /// <summary>Renderiza QR Code como grade de quadrados vetoriais.</summary>
    internal static void DrawQrCode(
        XGraphics gfx,
        string data,
        double x, double y,
        double size,
        XBrush darkBrush)
    {
        ArgumentNullException.ThrowIfNull(gfx);
        ArgumentNullException.ThrowIfNull(data);

        var barcode = QrEncoder.Encode(data, ErrorCorrectionLevel.M, Barcoder.Qr.Encoding.Auto);
        int totalModules = barcode.Bounds.X;

        double moduleSize = Math.Floor(size / totalModules);
        double offset = (size - moduleSize * totalModules) / 2;

        for (int c = 0; c < totalModules; c++)
        {
            for (int r = 0; r < totalModules; r++)
            {
                if (barcode.At(c, r))
                {
                    gfx.DrawRectangle(darkBrush,
                        x + offset + c * moduleSize,
                        y + offset + r * moduleSize,
                        moduleSize,
                        moduleSize);
                }
            }
        }
    }
}
