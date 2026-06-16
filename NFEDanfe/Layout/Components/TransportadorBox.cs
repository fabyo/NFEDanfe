using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using NFEDanfe.Domain.Formatting;
using NFEDanfe.Domain.Models;
using NFEDanfe.Layout.Configuration;

namespace NFEDanfe.Layout.Components;

public class TransportadorBox : IComponent
{
    private readonly TransportadorModel _transportador;

    public TransportadorBox(TransportadorModel transportador, bool isLandscape = false)
    {
        _transportador = transportador;
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
                .Text("TRANSPORTADOR / VOLUMES TRANSPORTADOS")
                .FontFamily(DanfeTheme.FontePadrao)
                .FontSize(DanfeTheme.TamanhoFonteLabel + 1f)
                .Bold();

            column.Item().Row(row =>
            {
                row.RelativeItem(5).LabelValueCell("RAZÃO SOCIAL / NOME", _transportador.RazaoSocial.ToUpper(), true, top: false);
                row.RelativeItem(2).LabelValueCell("FRETE POR CONTA", _transportador.FretePorConta.ToUpper(), true, top: false, left: false);
                row.RelativeItem(1).LabelValueCell("CÓDIGO ANTT", _transportador.CodigoAntt.ToUpper(), true, top: false, left: false);
                row.RelativeItem(1).LabelValueCell("PLACA DO VEÍCULO", _transportador.PlacaVeiculo.ToUpper(), true, top: false, left: false);
                row.RelativeItem(1).LabelValueCell("UF", _transportador.UfPlaca.ToUpper(), true, top: false, left: false);
                row.RelativeItem(2).LabelValueCell("CNPJ / CPF", DocumentFormatter.CnpjCpf(_transportador.Documento), true, top: false, left: false);
            });

            column.Item().Row(row =>
            {
                row.RelativeItem(5).LabelValueCell("ENDEREÇO", _transportador.EnderecoCompleto.ToUpper(), true, top: false);
                row.RelativeItem(4).LabelValueCell("MUNICÍPIO", _transportador.Municipio.ToUpper(), true, top: false, left: false);
                row.RelativeItem(1).LabelValueCell("UF", _transportador.Uf.ToUpper(), true, top: false, left: false);
                row.RelativeItem(2).LabelValueCell("INSCRIÇÃO ESTADUAL", _transportador.InscricaoEstadual.ToUpper(), true, top: false, left: false);
            });

            column.Item().Row(row =>
            {
                row.RelativeItem(1.5f).LabelValueCell("QUANTIDADE", DocumentFormatter.Decimal(_transportador.QuantidadeVolumes), true, top: false, alignRightValue: true);
                row.RelativeItem(2).LabelValueCell("ESPÉCIE", _transportador.Especie.ToUpper(), true, top: false, left: false);
                row.RelativeItem(2).LabelValueCell("MARCA", _transportador.Marca.ToUpper(), true, top: false, left: false);
                row.RelativeItem(1.5f).LabelValueCell("NUMERAÇÃO", _transportador.Numeracao.ToUpper(), true, top: false, left: false);
                row.RelativeItem(2.5f).LabelValueCell("PESO BRUTO", DocumentFormatter.Decimal(_transportador.PesoBruto), true, top: false, left: false, alignRightValue: true);
                row.RelativeItem(2.5f).LabelValueCell("PESO LÍQUIDO", DocumentFormatter.Decimal(_transportador.PesoLiquido), true, top: false, left: false, alignRightValue: true);
            });
        });
    }
}

public static class TransportadorBoxExtensions
{
    public static void TransportadorBox(this IContainer container, TransportadorModel transportador, bool isLandscape = false)
    {
        container.Component(new TransportadorBox(transportador, isLandscape));
    }
}
