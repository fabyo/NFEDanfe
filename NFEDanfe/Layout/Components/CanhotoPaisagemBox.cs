using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using NFEDanfe.Domain.Formatting;
using NFEDanfe.Domain.Models;
using NFEDanfe.Layout.Configuration;

namespace NFEDanfe.Layout.Components;

public class CanhotoPaisagemBox : IComponent
{
    private readonly Emitente _emitente;
    private readonly DadosDanfe _dados;
    private readonly Destinatario _destinatario;
    private readonly decimal _valorTotal;

    public CanhotoPaisagemBox(Emitente emitente, DadosDanfe dados, Destinatario destinatario, decimal valorTotal)
    {
        _emitente = emitente;
        _dados = dados;
        _destinatario = destinatario;
        _valorTotal = valorTotal;
    }

    public void Compose(IContainer container)
    {
        string receiptText =
            $"RECEBEMOS DE {_emitente.RazaoSocial.ToUpper()} OS PRODUTOS/SERVIÇOS DA NOTA FISCAL ELETRÔNICA. " +
            $"EMISSÃO: {_dados.DataEmissao:dd/MM/yyyy}  VALOR: R$ {DocumentFormatter.Money(_valorTotal)}  " +
            $"DEST.: {_destinatario.RazaoSocial.ToUpper()} - {DocumentFormatter.FullAddress(_destinatario.Endereco).ToUpper()}";

        container.Table(table =>
        {
            table.ColumnsDefinition(cols => cols.RelativeColumn());

            table.Cell().Border(DanfeTheme.EspessuraBorda).BorderColor(DanfeTheme.CorBorda)
                .Padding(2)
                .Column(c =>
                {
                    c.Item().AlignCenter().Text("NF-e").FontFamily(DanfeTheme.FontePadrao).FontSize(6f).Bold();
                    c.Item().AlignCenter().Text($"Nº {_dados.Numero}").FontFamily(DanfeTheme.FontePadrao).FontSize(4.5f).Bold();
                    c.Item().AlignCenter().Text($"Série {_dados.Serie:000}").FontFamily(DanfeTheme.FontePadrao).FontSize(4f);
                });

            table.Cell().Border(DanfeTheme.EspessuraBorda).BorderColor(DanfeTheme.CorBorda)
                .Padding(2)
                .Text(receiptText)
                .FontFamily(DanfeTheme.FontePadrao)
                .FontSize(3.5f)
                .LineHeight(1.1f);

            table.Cell().Border(DanfeTheme.EspessuraBorda).BorderColor(DanfeTheme.CorBorda)
                .Height(20)
                .Padding(2)
                .Text("DATA DE RECEBIMENTO")
                .FontFamily(DanfeTheme.FontePadrao)
                .FontSize(3.5f);

            table.Cell().Border(DanfeTheme.EspessuraBorda).BorderColor(DanfeTheme.CorBorda)
                .Height(20)
                .Padding(2)
                .Text("IDENTIFICAÇÃO E ASSINATURA DO RECEBEDOR")
                .FontFamily(DanfeTheme.FontePadrao)
                .FontSize(3.5f);
        });
    }
}

public static class CanhotoPaisagemBoxExtensions
{
    public static void CanhotoPaisagemBox(this IContainer container, Emitente emitente, DadosDanfe dados, Destinatario destinatario, decimal valorTotal)
    {
        container.Component(new CanhotoPaisagemBox(emitente, dados, destinatario, valorTotal));
    }
}
