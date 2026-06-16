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

    public ChaveAcessoBox(DadosDanfe dados)
    {
        _dados = dados;
    }

    public void Compose(IContainer container)
    {
        container
            .Border(DanfeTheme.EspessuraBorda)
            .BorderColor(DanfeTheme.CorBorda)
            .PaddingVertical(3)
            .PaddingHorizontal(4)
            .Column(column =>
            {
                column.Item().Height(28).Element(ComposeBarcode);

                column.Item().PaddingTop(6).Column(c =>
                {
                    c.Item().Text("CHAVE DE ACESSO")
                        .FontFamily(DanfeTheme.FontePadrao)
                        .FontSize(DanfeTheme.TamanhoFonteLabel);

                    c.Item().Text(FormatarChaveAcesso(_dados.ChaveAcesso))
                        .FontFamily(DanfeTheme.FontePadrao)
                        .FontSize(DanfeTheme.TamanhoFonteChaveAcesso)
                        .Bold();
                });

                column.Item().PaddingTop(4).LineHorizontal(0.5f).LineColor(DanfeTheme.CorBorda);
                column.Item().PaddingTop(2).Text("Consulta de autenticidade no portal nacional da NF-e\nwww.nfe.fazenda.gov.br/portal ou no site da Sefaz Autorizadora")
                    .FontFamily(DanfeTheme.FontePadrao)
                    .FontSize(DanfeTheme.TamanhoFonteSubtitulo)
                    .AlignCenter()
                    .LineHeight(1.1f);
            });
    }

    private void ComposeBarcode(IContainer container)
    {
        try
        {
            string chaveLimpa = Regex.Replace(_dados.ChaveAcesso, @"[^\d]", "");
            IReadOnlyList<BarcodeBar> bars = BarcodeEncoders.Code128.EncodeCode128(chaveLimpa);

            container.PaddingHorizontal(10).Row(row =>
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
    public static void ChaveAcessoBox(this IContainer container, DadosDanfe dados)
    {
        container.Component(new ChaveAcessoBox(dados));
    }
}
