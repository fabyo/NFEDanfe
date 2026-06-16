using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using NFEDanfe.Domain.Models;
using NFEDanfe.Layout.Configuration;

namespace NFEDanfe.Layout.Components;

public class DanfeBox : IComponent
{
    private readonly DadosDanfe _dados;

    public DanfeBox(DadosDanfe dados)
    {
        _dados = dados;
    }

    public void Compose(IContainer container)
    {
        container
            .BorderTop(DanfeTheme.EspessuraBorda)
            .BorderBottom(DanfeTheme.EspessuraBorda)
            .BorderColor(DanfeTheme.CorBorda)
            .PaddingVertical(3)
            .PaddingHorizontal(4)
            .Column(column =>
            {
                column.Item().AlignCenter().Text("DANFE")
                    .FontFamily(DanfeTheme.FontePadrao)
                    .FontSize(DanfeTheme.TamanhoFonteTituloDanfe)
                    .Bold();

                column.Item().AlignCenter().Text("Documento Auxiliar da\nNota Fiscal Eletrônica")
                    .FontFamily(DanfeTheme.FontePadrao)
                    .FontSize(DanfeTheme.TamanhoFonteSubtitulo)
                    .AlignCenter()
                    .LineHeight(1.1f);

                column.Item().PaddingVertical(4).Row(row =>
                {
                    row.RelativeItem()
                        .AlignLeft()
                        .AlignMiddle()
                        .Text("0 - Entrada\n1 - Saída")
                        .FontFamily(DanfeTheme.FontePadrao)
                        .FontSize(DanfeTheme.TamanhoFonteLabel)
                        .LineHeight(1.2f);

                    row.ConstantItem(15)
                        .PaddingLeft(3)
                        .Border(DanfeTheme.EspessuraBorda)
                        .BorderColor(DanfeTheme.CorBorda)
                        .Background(DanfeTheme.CorFundoTabela)
                        .AlignCenter()
                        .AlignMiddle()
                        .Text(_dados.TipoOperacao.ToString())
                        .FontFamily(DanfeTheme.FontePadrao)
                        .FontSize(DanfeTheme.TamanhoFonteValorDestaque)
                        .Bold();
                });

                column.Item().AlignCenter().Text($"Nº {_dados.Numero}")
                    .FontFamily(DanfeTheme.FontePadrao)
                    .FontSize(DanfeTheme.TamanhoFonteValor)
                    .Bold();

                column.Item().AlignCenter().Text($"SÉRIE {_dados.Serie:000}")
                    .FontFamily(DanfeTheme.FontePadrao)
                    .FontSize(DanfeTheme.TamanhoFonteValor)
                    .Bold();

                column.Item().AlignCenter().Text(text =>
                {
                    text.DefaultTextStyle(TextStyle.Default.FontFamily(DanfeTheme.FontePadrao).FontSize(DanfeTheme.TamanhoFonteValor));
                    text.Span("FOLHA: ");
                    text.CurrentPageNumber();
                    text.Span("/");
                    text.TotalPages();
                });
            });
    }
}

public static class DanfeBoxExtensions
{
    public static void DanfeBox(this IContainer container, DadosDanfe dados)
    {
        container.Component(new DanfeBox(dados));
    }
}
