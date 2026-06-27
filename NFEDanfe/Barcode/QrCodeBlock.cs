using System;
using PdfSharp.Drawing;

namespace NFEDanfe.Barcode;

/// <summary>Componente que injeta o QR Code no bloco do emitente.</summary>
internal sealed class QrCodeBlock
{
    private readonly string _urlQrCode;

    /// <summary>Cria o bloco de QR Code com a URL.</summary>
    internal QrCodeBlock(string urlQrCode)
    {
        ArgumentNullException.ThrowIfNull(urlQrCode);
        _urlQrCode = urlQrCode;
    }

    /// <summary>Desenha o QR Code na área reservada.</summary>
    internal void Draw(XGraphics gfx, DanfeStyleCatalog styles,
                       double x, double y, double size)
    {
        if (string.IsNullOrEmpty(_urlQrCode)) return;

        BarcodeRenderer.DrawQrCode(gfx, _urlQrCode, x, y, size, styles.TextBrush);
    }
}
