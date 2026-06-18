using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using NFEDanfe.Domain.Formatting;
using NFEDanfe.Domain.Models;
using NFEDanfe.Layout.Configuration;

namespace NFEDanfe.Layout.Components;

public class ImpostosBox : IComponent
{
    private readonly ImpostosModel _impostos;

    public ImpostosBox(ImpostosModel impostos, bool isLandscape = false)
    {
        _impostos = impostos;
    }

    public void Compose(IContainer container)
    {
        container.Column(column =>
        {
            column.Item().Background(Colors.Grey.Lighten3)
                .Border(DanfeTheme.EspessuraBorda)
                .BorderColor(DanfeTheme.CorBorda)
                .PaddingHorizontal(4)
                .PaddingVertical(1)
                .Row(row =>
                {
                    row.RelativeItem().Text("CÁLCULO DO IMPOSTO")
                        .FontFamily(DanfeTheme.FontePadrao)
                        .FontSize(DanfeTheme.TamanhoFonteLabel + 1f)
                        .Bold();

                    if (_impostos.ValorTotTrib > 0)
                    {
                        row.AutoItem().Text(text =>
                        {
                            text.Span("TOTAL DE IMPOSTOS: ")
                                .FontFamily(DanfeTheme.FontePadrao)
                                .FontSize(DanfeTheme.TamanhoFonteLabel + 1f);

                            text.Span(DocumentFormatter.Money(_impostos.ValorTotTrib))
                                .FontFamily(DanfeTheme.FontePadrao)
                                .FontSize(DanfeTheme.TamanhoFonteLabel + 1f)
                                .Bold();
                        });
                    }
                });

            column.Item().Row(row =>
            {
                AddMoneyCell(row, "BASE DE CÁLCULO DO ICMS", _impostos.BaseCalculoIcms, left: true);
                AddMoneyCell(row, "VALOR DO ICMS", _impostos.ValorIcms);
                AddMoneyCell(row, "BASE DE CÁLCULO DO ICMS ST", _impostos.BaseCalculoIcmsSt);
                AddMoneyCell(row, "VALOR DO ICMS ST", _impostos.ValorIcmsSt);
                AddMoneyCell(row, "FCP", _impostos.ValorFcp ?? 0m);
                AddMoneyCell(row, "VALOR IMP. IMPORTAÇÃO", _impostos.ValorIi);
                AddMoneyCell(row, "VALOR ICMS UF REMET.", _impostos.ValorIcmsUfRemet);
                AddMoneyCell(row, "VALOR DO PIS", _impostos.ValorPis);
                AddMoneyCell(row, "VALOR TOTAL DOS PRODUTOS", _impostos.ValorProdutos);
            });

            column.Item().Row(row =>
            {
                AddMoneyCell(row, "VALOR DO FRETE", _impostos.ValorFrete, left: true);
                AddMoneyCell(row, "VALOR DO SEGURO", _impostos.ValorSeguro);
                AddMoneyCell(row, "DESCONTO", _impostos.ValorDesconto);
                AddMoneyCell(row, "OUTRAS DESPESAS ACESSÓRIAS", _impostos.OutrasDespesas);
                AddMoneyCell(row, "VALOR TOTAL IPI", _impostos.ValorIpi);
                AddMoneyCell(row, "VALOR ICMS UF DEST", _impostos.ValorIcmsUfDest);
                AddMoneyCell(row, "VALOR TOTAL TRIB.", _impostos.ValorTotTrib);
                AddMoneyCell(row, "VALOR DA COFINS", _impostos.ValorCofins);
                AddMoneyCell(row, "VALOR TOTAL DA NOTA", _impostos.ValorNota);
            });
        });
    }

    private static void AddBlankCell(RowDescriptor row)
    {
        row.RelativeItem(2).LabelValueCell(
            label: "",
            value: "",
            boldValue: false,
            top: false,
            left: false);
    }

    private static void AddMoneyCell(RowDescriptor row, string label, decimal value, bool left = false)
    {
        row.RelativeItem(2).LabelValueCell(
            label: label,
            value: DocumentFormatter.Money(value),
            boldValue: true,
            top: false,
            left: left,
            alignRightValue: true);
    }
}

public static class ImpostosBoxExtensions
{
    public static void ImpostosBox(this IContainer container, ImpostosModel impostos, bool isLandscape = false)
    {
        container.Component(new ImpostosBox(impostos, isLandscape));
    }
}
