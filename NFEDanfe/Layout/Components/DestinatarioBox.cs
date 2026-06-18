using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using NFEDanfe.Domain.Formatting;
using NFEDanfe.Domain.Models;
using NFEDanfe.Layout.Configuration;

namespace NFEDanfe.Layout.Components;

public class DestinatarioBox : IComponent
{
    private readonly Destinatario _destinatario;
    private readonly DadosDanfe _dados;
    private readonly bool _isLandscape;

    public DestinatarioBox(Destinatario destinatario, DadosDanfe dados, bool isLandscape = false)
    {
        _destinatario = destinatario;
        _dados = dados;
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
                .Text("DESTINATÁRIO / REMETENTE")
                .FontFamily(DanfeTheme.FontePadrao)
                .FontSize(DanfeTheme.TamanhoFonteLabel + 1f)
                .Bold();

            column.Item().Row(row =>
            {
                row.RelativeItem(_isLandscape ? 7.5f : 7f).LabelValueCell(
                    label: "NOME / RAZÃO SOCIAL",
                    value: _destinatario.RazaoSocial.ToUpper(),
                    boldValue: true);

                row.RelativeItem(3).LabelValueCell(
                    label: "CNPJ / CPF",
                    value: DocumentFormatter.CnpjCpf(_destinatario.Documento),
                    boldValue: true,
                    left: false);

                row.RelativeItem(_isLandscape ? 1.5f : 2f).LabelValueCell(
                    label: "DATA DA EMISSÃO",
                    value: _dados.DataEmissao.ToString("dd/MM/yyyy"),
                    boldValue: true,
                    left: false);
            });

            column.Item().Row(row =>
            {
                row.RelativeItem(_isLandscape ? 6.5f : 6f).LabelValueCell(
                    label: "ENDEREÇO",
                    value: DocumentFormatter.Address(_destinatario.Endereco).ToUpper(),
                    boldValue: true,
                    top: false);

                row.RelativeItem(2).LabelValueCell(
                    label: "BAIRRO / DISTRITO",
                    value: _destinatario.Endereco.Bairro.ToUpper(),
                    boldValue: true,
                    top: false,
                    left: false);

                row.RelativeItem(2).LabelValueCell(
                    label: "CEP",
                    value: DocumentFormatter.Cep(_destinatario.Endereco.Cep),
                    boldValue: true,
                    top: false,
                    left: false);

                row.RelativeItem(_isLandscape ? 1.5f : 2f).LabelValueCell(
                    label: "DATA DA ENTRADA / SAÍDA",
                    value: _dados.DataEntradaSaida?.ToString("dd/MM/yyyy") ?? string.Empty,
                    boldValue: true,
                    top: false,
                    left: false);
            });

            column.Item().Row(row =>
            {
                row.RelativeItem(_isLandscape ? 5.5f : 5f).LabelValueCell(
                    label: "MUNICÍPIO",
                    value: _destinatario.Endereco.Municipio.ToUpper(),
                    boldValue: true,
                    top: false);

                row.RelativeItem(2).LabelValueCell(
                    label: "FONE / FAX",
                    value: DocumentFormatter.Phone(_destinatario.Telefone),
                    boldValue: true,
                    top: false,
                    left: false);

                row.RelativeItem(1).LabelValueCell(
                    label: "UF",
                    value: _destinatario.Endereco.Uf.ToUpper(),
                    boldValue: true,
                    top: false,
                    left: false);

                row.RelativeItem(2).LabelValueCell(
                    label: "INSCRIÇÃO ESTADUAL",
                    value: _destinatario.InscricaoEstadual.ToUpper(),
                    boldValue: true,
                    top: false,
                    left: false);

                row.RelativeItem(_isLandscape ? 1.5f : 2f).LabelValueCell(
                    label: "HORA DA SAÍDA",
                    value: _dados.DataEntradaSaida?.ToString("HH:mm:ss") ?? string.Empty,
                    boldValue: true,
                    top: false,
                    left: false);
            });
        });
    }
}

public static class DestinatarioBoxExtensions
{
    public static void DestinatarioBox(this IContainer container, Destinatario destinatario, DadosDanfe dados, bool isLandscape = false)
    {
        container.Component(new DestinatarioBox(destinatario, dados, isLandscape));
    }
}
