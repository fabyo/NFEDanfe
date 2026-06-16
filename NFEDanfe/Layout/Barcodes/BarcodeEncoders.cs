namespace NFEDanfe.Layout.Barcodes;

internal static class BarcodeEncoders
{
    public static IBarcodeEncoder Code128 { get; } = new BarcoderCode128Encoder();
}
