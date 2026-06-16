namespace NFEDanfe.Layout.Barcodes;

internal interface IBarcodeEncoder
{
    IReadOnlyList<BarcodeBar> EncodeCode128(string value);
}
