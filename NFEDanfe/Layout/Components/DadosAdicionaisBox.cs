using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using NFEDanfe.Domain.Models;
using NFEDanfe.Layout.Configuration;

namespace NFEDanfe.Layout.Components;

public class DadosAdicionaisBox : IComponent
{
    private readonly DadosAdicionaisModel _dados;

    public DadosAdicionaisBox(DadosAdicionaisModel dados)
    {
        _dados = dados;
    }

    public void Compose(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Background(Colors.Grey.Lighten3)
                .Border(DanfeTheme.EspessuraBorda)
                .BorderColor(DanfeTheme.CorBorda)
                .PaddingLeft(4)
                .PaddingVertical(1)
                .Text("DADOS ADICIONAIS")
                .FontFamily(DanfeTheme.FontePadrao)
                .FontSize(DanfeTheme.TamanhoFonteLabel + 1f)
                .Bold();

            column.Item().MinHeight(70).Row(row =>
            {
                row.RelativeItem(8)
                    .BorderLeft(DanfeTheme.EspessuraBorda)
                    .BorderBottom(DanfeTheme.EspessuraBorda)
                    .BorderColor(DanfeTheme.CorBorda)
                    .Padding(4)
                    .Column(c =>
                    {
                        c.Item().Text("INFORMAÇÕES COMPLEMENTARES")
                            .FontFamily(DanfeTheme.FontePadrao)
                            .FontSize(DanfeTheme.TamanhoFonteLabel)
                            .Bold();

                        c.Item().PaddingTop(2)
                            .Text(_dados.InformacoesComplementares ?? string.Empty)
                            .FontFamily(DanfeTheme.FontePadrao)
                            .FontSize(DanfeTheme.TamanhoFonteSubtitulo);
                    });

                row.RelativeItem(4)
                    .BorderLeft(DanfeTheme.EspessuraBorda)
                    .BorderRight(DanfeTheme.EspessuraBorda)
                    .BorderBottom(DanfeTheme.EspessuraBorda)
                    .BorderColor(DanfeTheme.CorBorda)
                    .Padding(4)
                    .Column(c =>
                    {
                        c.Item().Text("RESERVADO AO FISCO")
                            .FontFamily(DanfeTheme.FontePadrao)
                            .FontSize(DanfeTheme.TamanhoFonteLabel)
                            .Bold();

                        c.Item().PaddingTop(2)
                            .Text(_dados.InformacoesFisco ?? string.Empty)
                            .FontFamily(DanfeTheme.FontePadrao)
                            .FontSize(DanfeTheme.TamanhoFonteSubtitulo);
                    });
            });
        });
    }
}

public static class DadosAdicionaisBoxExtensions
{
    public static void DadosAdicionaisBox(this IContainer container, DadosAdicionaisModel dados)
    {
        container.Component(new DadosAdicionaisBox(dados));
    }
}
