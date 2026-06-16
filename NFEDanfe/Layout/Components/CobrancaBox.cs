using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using NFEDanfe.Domain.Formatting;
using NFEDanfe.Domain.Models;
using NFEDanfe.Layout.Configuration;

namespace NFEDanfe.Layout.Components;

public class CobrancaBox : IComponent
{
    private readonly CobrancaModel _cobranca;

    public CobrancaBox(CobrancaModel cobranca)
    {
        _cobranca = cobranca;
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
                .Text("FATURA / DUPLICATAS")
                .FontFamily(DanfeTheme.FontePadrao)
                .FontSize(DanfeTheme.TamanhoFonteLabel + 1f)
                .Bold();

            if (_cobranca.NumeroFatura != null || _cobranca.ValorOriginal.HasValue)
            {
                column.Item().Row(row =>
                {
                    row.RelativeItem(3).LabelValueCell("NÚMERO DA FATURA", _cobranca.NumeroFatura ?? string.Empty, true, top: false);
                    row.RelativeItem(3).LabelValueCell("VALOR ORIGINAL", FormatMoney(_cobranca.ValorOriginal), true, top: false, left: false, alignRightValue: true);
                    row.RelativeItem(3).LabelValueCell("VALOR DESCONTO", FormatMoney(_cobranca.ValorDesconto), true, top: false, left: false, alignRightValue: true);
                    row.RelativeItem(3).LabelValueCell("VALOR LÍQUIDO", FormatMoney(_cobranca.ValorLiquido), true, top: false, left: false, alignRightValue: true);
                });
            }

            if (_cobranca.Duplicatas.Any())
            {
                column.Item().PaddingTop(4).Inlined(inlined =>
                {
                    inlined.Spacing(4);

                    foreach (DuplicataModel dup in _cobranca.Duplicatas)
                    {
                        inlined.Item().Element(x => ComposeDuplicataCard(x, dup));
                    }
                });
            }
        });
    }

    private static void ComposeDuplicataCard(IContainer container, DuplicataModel dup)
    {
        container
            .Width(90)
            .Border(DanfeTheme.EspessuraBorda)
            .BorderColor(DanfeTheme.CorBorda)
            .Background(DanfeTheme.CorFundoTabela)
            .PaddingHorizontal(4)
            .PaddingVertical(2)
            .Column(column =>
            {
                CardRow(column, "DUP:", dup.Numero);
                CardRow(column, "VENC:", dup.Vencimento == DateTime.MinValue ? string.Empty : dup.Vencimento.ToString("dd/MM/yyyy"));
                CardRow(column, "VALOR:", $"R$ {DocumentFormatter.Money(dup.Valor)}");
            });
    }

    private static void CardRow(ColumnDescriptor column, string label, string value)
    {
        column.Item().Row(row =>
        {
            row.AutoItem().Text(label)
                .FontFamily(DanfeTheme.FontePadrao)
                .FontSize(DanfeTheme.TamanhoFonteLabel);

            row.RelativeItem().AlignRight().Text(value)
                .FontFamily(DanfeTheme.FontePadrao)
                .FontSize(DanfeTheme.TamanhoFonteValor)
                .Bold();
        });
    }

    private static string FormatMoney(decimal? value)
    {
        return value.HasValue ? $"R$ {DocumentFormatter.Money(value.Value)}" : string.Empty;
    }
}

public static class CobrancaBoxExtensions
{
    public static void CobrancaBox(this IContainer container, CobrancaModel cobranca)
    {
        container.Component(new CobrancaBox(cobranca));
    }
}
