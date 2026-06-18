using QuestPDF.Helpers;

namespace NFEDanfe.Layout.Configuration;

public static class DanfeTheme
{
    [System.ThreadStatic]
    private static string? _fontePadrao;

    public static string FontePadrao
    {
        get => _fontePadrao ?? Fonts.Arial;
        set => _fontePadrao = value;
    }

    public const float TamanhoFonteLabel = 5f;
    public const float TamanhoFonteValor = 8f;
    public const float TamanhoFonteValorDestaque = 9f;
    public const float TamanhoFonteTituloDanfe = 11f;
    public const float TamanhoFonteSubtitulo = 6f;
    public const float TamanhoFonteChaveAcesso = 8f;

    public const float EspessuraBorda = 0.8f;
    public const float PaddingInternoHorizontal = 4f;

    [System.ThreadStatic]
    private static float? _paddingInternoVertical;

    public static float PaddingInternoVertical
    {
        get => _paddingInternoVertical ?? 2f;
        set => _paddingInternoVertical = value;
    }

    public static readonly string CorBorda = Colors.Black;
    public static readonly string CorTexto = Colors.Black;
    public static readonly string CorFundoTabela = Colors.White;
}
