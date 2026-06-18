using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using NFEDanfe.Domain.Formatting;
using NFEDanfe.Domain.Models;
using NFEDanfe.Layout.Configuration;

namespace NFEDanfe.Layout.Components;

public class LocalEntregaBox : IComponent
{
    private readonly LocalEntrega _entrega;
    private readonly bool _isLandscape;

    public LocalEntregaBox(LocalEntrega entrega, bool isLandscape = false)
    {
        _entrega = entrega;
        _isLandscape = isLandscape;
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
                .Text("INFORMAÇÕES DO LOCAL DE ENTREGA")
                .FontFamily(DanfeTheme.FontePadrao)
                .FontSize(DanfeTheme.TamanhoFonteLabel + 1f)
                .Bold();

            column.Item().Row(row =>
            {
                row.RelativeItem(_isLandscape ? 7.5f : 7f).LabelValueCell(
                    label: "NOME / RAZÃO SOCIAL",
                    value: _entrega.RazaoSocial?.ToUpper() ?? string.Empty,
                    boldValue: true);

                row.RelativeItem(3).LabelValueCell(
                    label: "CNPJ / CPF",
                    value: DocumentFormatter.CnpjCpf(_entrega.Documento),
                    boldValue: true,
                    left: false);

                row.RelativeItem(_isLandscape ? 1.5f : 2f).LabelValueCell(
                    label: "INSCRIÇÃO ESTADUAL",
                    value: _entrega.InscricaoEstadual?.ToUpper() ?? string.Empty,
                    boldValue: true,
                    left: false);
            });

            column.Item().Row(row =>
            {
                row.RelativeItem(_isLandscape ? 6.5f : 6f).LabelValueCell(
                    label: "ENDEREÇO",
                    value: DocumentFormatter.Address(_entrega.Endereco).ToUpper(),
                    boldValue: true,
                    top: false);

                row.RelativeItem(3).LabelValueCell(
                    label: "BAIRRO / DISTRITO",
                    value: _entrega.Endereco.Bairro.ToUpper(),
                    boldValue: true,
                    top: false,
                    left: false);

                row.RelativeItem(3).LabelValueCell(
                    label: "CEP",
                    value: DocumentFormatter.Cep(_entrega.Endereco.Cep),
                    boldValue: true,
                    top: false,
                    left: false);
            });

            column.Item().Row(row =>
            {
                row.RelativeItem(_isLandscape ? 8.5f : 7f).LabelValueCell(
                    label: "MUNICÍPIO",
                    value: _entrega.Endereco.Municipio.ToUpper(),
                    boldValue: true,
                    top: false);

                row.RelativeItem(1).LabelValueCell(
                    label: "UF",
                    value: _entrega.Endereco.Uf.ToUpper(),
                    boldValue: true,
                    top: false,
                    left: false);

                row.RelativeItem(4).LabelValueCell(
                    label: "FONE / FAX",
                    value: DocumentFormatter.Phone(_entrega.Telefone),
                    boldValue: true,
                    top: false,
                    left: false);
            });
        });
    }
}

public static class LocalEntregaBoxExtensions
{
    public static void LocalEntregaBox(this IContainer container, LocalEntrega entrega, bool isLandscape = false)
    {
        container.Component(new LocalEntregaBox(entrega, isLandscape));
    }
}
