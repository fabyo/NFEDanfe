using Barcoder.Code128;

namespace NFEDanfe.Layout.Barcodes;

internal sealed class BarcoderCode128Encoder : IBarcodeEncoder
{
    public IReadOnlyList<BarcodeBar> EncodeCode128(string value)
    {
        Barcoder.IBarcode barcode = Code128Encoder.Encode(value);
        int barcodeWidth = barcode.Bounds.X;
        List<BarcodeBar> bars = [];

        int x = 0;
        while (x < barcodeWidth)
        {
            bool color = barcode.At(x, 0);
            int count = 0;

            while (x < barcodeWidth && barcode.At(x, 0) == color)
            {
                count++;
                x++;
            }

            bars.Add(new BarcodeBar(color, count));
        }

        return bars;
    }
}
