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
                .PaddingLeft(4)
                .PaddingVertical(1)
                .Text("CÁLCULO DO IMPOSTO")
                .FontFamily(DanfeTheme.FontePadrao)
                .FontSize(DanfeTheme.TamanhoFonteLabel + 1f)
                .Bold();

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
        string formattedValue = DocumentFormatter.Money(value);

        row.RelativeItem(2)
            .BorderTop(0)
            .BorderLeft(left ? DanfeTheme.EspessuraBorda : 0)
            .BorderBottom(DanfeTheme.EspessuraBorda)
            .BorderRight(DanfeTheme.EspessuraBorda)
            .BorderColor(DanfeTheme.CorBorda)
            .PaddingHorizontal(DanfeTheme.PaddingInternoHorizontal)
            .PaddingVertical(DanfeTheme.PaddingInternoVertical)
            .Layers(layers =>
            {
                layers.PrimaryLayer().Column(column =>
                {
                    column.Item().Text(label.ToUpper())
                        .FontFamily(DanfeTheme.FontePadrao)
                        .FontSize(DanfeTheme.TamanhoFonteLabel)
                        .FontColor(Colors.Transparent);

                    column.Item().PaddingTop(1).AlignRight().Text(formattedValue)
                        .FontFamily(DanfeTheme.FontePadrao)
                        .FontSize(DanfeTheme.TamanhoFonteValor)
                        .FontColor(Colors.Transparent)
                        .Bold();
                });

                layers.Layer().AlignTop().Text(label.ToUpper())
                    .FontFamily(DanfeTheme.FontePadrao)
                    .FontSize(DanfeTheme.TamanhoFonteLabel);

                layers.Layer().AlignBottom().AlignRight().Text(formattedValue)
                    .FontFamily(DanfeTheme.FontePadrao)
                    .FontSize(DanfeTheme.TamanhoFonteValor)
                    .Bold();
            });
    }
}

public static class ImpostosBoxExtensions
{
    public static void ImpostosBox(this IContainer container, ImpostosModel impostos, bool isLandscape = false)
    {
        container.Component(new ImpostosBox(impostos, isLandscape));
    }
}
