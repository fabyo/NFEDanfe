using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using NFEDanfe.Domain.Formatting;
using NFEDanfe.Domain.Models;
using NFEDanfe.Layout.Configuration;

namespace NFEDanfe.Layout.Components;

public class CanhotoBox : IComponent
{
    private readonly Emitente _emitente;
    private readonly DadosDanfe _dados;
    private readonly Destinatario _destinatario;
    private readonly decimal _valorTotal;

    public CanhotoBox(Emitente emitente, DadosDanfe dados, Destinatario destinatario, decimal valorTotal)
    {
        _emitente = emitente;
        _dados = dados;
        _destinatario = destinatario;
        _valorTotal = valorTotal;
    }

    public void Compose(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Row(row =>
            {
                row.RelativeItem(9.5f).Column(leftCol =>
                {
                    leftCol.Item()
                        .Border(DanfeTheme.EspessuraBorda)
                        .BorderColor(DanfeTheme.CorBorda)
                        .Padding(4)
                        .Text(text =>
                        {
                            text.Span("RECEBEMOS DE ").FontFamily(DanfeTheme.FontePadrao).FontSize(DanfeTheme.TamanhoFonteSubtitulo);
                            text.Span(_emitente.RazaoSocial.ToUpper()).FontFamily(DanfeTheme.FontePadrao).FontSize(DanfeTheme.TamanhoFonteSubtitulo).Bold();
                            text.Span(" OS PRODUTOS E/OU SERVIÇOS CONSTANTES DA NOTA FISCAL ELETRÔNICA INDICADA ABAIXO. ").FontFamily(DanfeTheme.FontePadrao).FontSize(DanfeTheme.TamanhoFonteSubtitulo);
                            text.Span("EMISSÃO: ").FontFamily(DanfeTheme.FontePadrao).FontSize(DanfeTheme.TamanhoFonteSubtitulo);
                            text.Span($"{_dados.DataEmissao:dd/MM/yyyy} ").FontFamily(DanfeTheme.FontePadrao).FontSize(DanfeTheme.TamanhoFonteSubtitulo).Bold();
                            text.Span("VALOR TOTAL: ").FontFamily(DanfeTheme.FontePadrao).FontSize(DanfeTheme.TamanhoFonteSubtitulo);
                            text.Span($"R$ {DocumentFormatter.Money(_valorTotal)} ").FontFamily(DanfeTheme.FontePadrao).FontSize(DanfeTheme.TamanhoFonteSubtitulo).Bold();
                            text.Span("DESTINATÁRIO: ").FontFamily(DanfeTheme.FontePadrao).FontSize(DanfeTheme.TamanhoFonteSubtitulo);
                            text.Span(_destinatario.RazaoSocial.ToUpper()).FontFamily(DanfeTheme.FontePadrao).FontSize(DanfeTheme.TamanhoFonteSubtitulo).Bold();
                            text.Span($" - {DocumentFormatter.FullAddress(_destinatario.Endereco)}").FontFamily(DanfeTheme.FontePadrao).FontSize(DanfeTheme.TamanhoFonteSubtitulo);
                        });

                    leftCol.Item().Row(fieldsRow =>
                    {
                        fieldsRow.RelativeItem(3)
                            .BorderLeft(DanfeTheme.EspessuraBorda)
                            .BorderBottom(DanfeTheme.EspessuraBorda)
                            .BorderRight(DanfeTheme.EspessuraBorda)
                            .BorderColor(DanfeTheme.CorBorda)
                            .Padding(3)
                            .Height(24)
                            .Text("DATA DE RECEBIMENTO")
                            .FontFamily(DanfeTheme.FontePadrao)
                            .FontSize(DanfeTheme.TamanhoFonteLabel);

                        fieldsRow.RelativeItem(9)
                            .BorderBottom(DanfeTheme.EspessuraBorda)
                            .BorderRight(DanfeTheme.EspessuraBorda)
                            .BorderColor(DanfeTheme.CorBorda)
                            .Padding(3)
                            .Height(24)
                            .Text("IDENTIFICAÇÃO E ASSINATURA DO RECEBEDOR")
                            .FontFamily(DanfeTheme.FontePadrao)
                            .FontSize(DanfeTheme.TamanhoFonteLabel);
                    });

                    leftCol.Item()
                        .Height(10)
                        .BorderLeft(DanfeTheme.EspessuraBorda)
                        .BorderRight(DanfeTheme.EspessuraBorda)
                        .BorderBottom(DanfeTheme.EspessuraBorda)
                        .BorderColor(DanfeTheme.CorBorda);
                });

                row.RelativeItem(2.5f)
                    .BorderTop(DanfeTheme.EspessuraBorda)
                    .BorderBottom(DanfeTheme.EspessuraBorda)
                    .BorderRight(DanfeTheme.EspessuraBorda)
                    .BorderColor(DanfeTheme.CorBorda)
                    .Padding(4)
                    .AlignMiddle()
                    .AlignCenter()
                    .Column(rightCol =>
                    {
                        rightCol.Item().AlignCenter().Text("NF-e").FontFamily(DanfeTheme.FontePadrao).FontSize(DanfeTheme.TamanhoFonteTituloDanfe).Bold();
                        rightCol.Item().AlignCenter().Text($"Nº {_dados.Numero}").FontFamily(DanfeTheme.FontePadrao).FontSize(DanfeTheme.TamanhoFonteValor).Bold();
                        rightCol.Item().AlignCenter().Text($"Série {_dados.Serie:000}").FontFamily(DanfeTheme.FontePadrao).FontSize(DanfeTheme.TamanhoFonteValor).Bold();
                    });
            });

            column.Item().PaddingVertical(4).LineHorizontal(0.5f).LineColor(Colors.Black).LineDashPattern([3f, 3f]);
        });
    }
}

public static class CanhotoBoxExtensions
{
    public static void CanhotoBox(this IContainer container, Emitente emitente, DadosDanfe dados, Destinatario destinatario, decimal valorTotal)
    {
        container.Component(new CanhotoBox(emitente, dados, destinatario, valorTotal));
    }
}
