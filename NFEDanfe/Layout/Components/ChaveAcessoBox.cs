using System.Text.RegularExpressions;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using NFEDanfe.Domain.Models;
using NFEDanfe.Layout.Barcodes;
using NFEDanfe.Layout.Configuration;

namespace NFEDanfe.Layout.Components;

public class ChaveAcessoBox : IComponent
{
    private readonly DadosDanfe _dados;
    private readonly bool _isLandscape;

    public ChaveAcessoBox(DadosDanfe dados, bool isLandscape = false)
    {
        _dados = dados;
        _isLandscape = isLandscape;
    }

    public void Compose(IContainer container)
    {
        container
            .Border(DanfeTheme.EspessuraBorda)
            .BorderColor(DanfeTheme.CorBorda)
            .PaddingVertical(_isLandscape ? 2 : 3)
            .Column(column =>
            {
                column.Item().Height(_isLandscape ? 22 : 28).Element(ComposeBarcode);

                column.Item().PaddingTop(_isLandscape ? 3 : 6).PaddingHorizontal(4).Column(c =>
                {
                    c.Item().Text("CHAVE DE ACESSO")
                        .FontFamily(DanfeTheme.FontePadrao)
                        .FontSize(DanfeTheme.TamanhoFonteLabel)
                        .FontColor(Colors.Grey.Darken2);

                    c.Item().PaddingTop(_isLandscape ? 1 : 2).AlignCenter().Text(FormatarChaveAcesso(_dados.ChaveAcesso))
                        .FontFamily(DanfeTheme.FontePadrao)
                        .FontSize(_isLandscape ? DanfeTheme.TamanhoFonteChaveAcesso - 0.5f : DanfeTheme.TamanhoFonteChaveAcesso)
                        .Bold();
                });

                column.Item().PaddingTop(_isLandscape ? 2 : 4).LineHorizontal(0.5f).LineColor(DanfeTheme.CorBorda);
                column.Item().PaddingTop(_isLandscape ? 1 : 2).PaddingHorizontal(4).Text("Consulta de autenticidade no portal nacional da NF-e\nwww.nfe.fazenda.gov.br/portal ou no site da Sefaz Autorizadora")
                    .FontFamily(DanfeTheme.FontePadrao)
                    .FontSize(_isLandscape ? DanfeTheme.TamanhoFonteSubtitulo - 0.5f : DanfeTheme.TamanhoFonteSubtitulo)
                    .AlignCenter()
                    .LineHeight(1.0f);
            });
    }

    private void ComposeBarcode(IContainer container)
    {
        try
        {
            string chaveLimpa = Regex.Replace(_dados.ChaveAcesso, @"[^\d]", "");
            IReadOnlyList<BarcodeBar> bars = BarcodeEncoders.Code128.EncodeCode128(chaveLimpa);

            container.PaddingHorizontal(14).Row(row =>
            {
                foreach (BarcodeBar bar in bars)
                {
                    row.RelativeItem(bar.Width).Background(bar.IsFilled ? Colors.Black : Colors.Transparent);
                }
            });
        }
        catch
        {
            container.Border(0.5f).BorderColor(Colors.Grey.Lighten2).AlignCenter().AlignMiddle()
                .Text("[ERROR GENERATING BARCODE]")
                .FontFamily(DanfeTheme.FontePadrao)
                .FontSize(DanfeTheme.TamanhoFonteLabel);
        }
    }

    private static string FormatarChaveAcesso(string chave)
    {
        if (string.IsNullOrWhiteSpace(chave))
        {
            return string.Empty;
        }

        string limpa = Regex.Replace(chave, @"[^\d]", "");
        if (limpa.Length != 44)
        {
            return chave;
        }

        string[] grupos = new string[11];
        for (int i = 0; i < grupos.Length; i++)
        {
            grupos[i] = limpa.Substring(i * 4, 4);
        }

        return string.Join(" ", grupos);
    }
}

public static class ChaveAcessoBoxExtensions
{
    public static void ChaveAcessoBox(this IContainer container, DadosDanfe dados, bool isLandscape = false)
    {
        container.Component(new ChaveAcessoBox(dados, isLandscape));
    }
}
