using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using NFEDanfe.Domain.Formatting;
using NFEDanfe.Domain.Models;
using NFEDanfe.Layout.Configuration;

namespace NFEDanfe.Layout.Components;

public class ProdutosBox : IComponent
{
    private static readonly float[] DashedGridPattern = [2f, 2f];
    private const float InternalGridThickness = 0.35f;

    private readonly IReadOnlyList<ProdutoModel> _produtos;
    private readonly bool _isLandscape;

    public ProdutosBox(IReadOnlyList<ProdutoModel>? produtos, bool isLandscape = false)
    {
        _produtos = produtos ?? [];
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
                .Text("DADOS DO PRODUTO / SERVIÇOS")
                .FontFamily(DanfeTheme.FontePadrao)
                .FontSize(DanfeTheme.TamanhoFonteLabel + 1f)
                .Bold();

            column.Item()
                .Border(DanfeTheme.EspessuraBorda)
                .BorderColor(DanfeTheme.CorBorda)
                .Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        if (_isLandscape)
                        {
                            columns.ConstantColumn(50); // Código
                            columns.RelativeColumn();   // Descrição
                            columns.ConstantColumn(45); // NCM
                            columns.ConstantColumn(30); // CST
                            columns.ConstantColumn(30); // CFOP
                            columns.ConstantColumn(28); // UN
                            columns.ConstantColumn(45); // Quant
                            columns.ConstantColumn(52); // Valor unit
                            columns.ConstantColumn(52); // Valor total
                            columns.ConstantColumn(48); // Valor desc
                            columns.ConstantColumn(52); // BC ICMS
                            columns.ConstantColumn(48); // Valor ICMS
                            columns.ConstantColumn(42); // Valor IPI
                            columns.ConstantColumn(30); // Alíq. ICMS
                            columns.ConstantColumn(30); // Alíq. IPI
                        }
                        else
                        {
                            columns.ConstantColumn(40); // Código
                            columns.RelativeColumn();   // Descrição
                            columns.ConstantColumn(34); // NCM
                            columns.ConstantColumn(22); // CST
                            columns.ConstantColumn(22); // CFOP
                            columns.ConstantColumn(19); // UN
                            columns.ConstantColumn(34); // Quant
                            columns.ConstantColumn(38); // Valor unit
                            columns.ConstantColumn(40); // Valor total
                            columns.ConstantColumn(36); // Valor desc
                            columns.ConstantColumn(38); // BC ICMS
                            columns.ConstantColumn(36); // Valor ICMS
                            columns.ConstantColumn(30); // Valor IPI
                            columns.ConstantColumn(21); // Alíq. ICMS
                            columns.ConstantColumn(21); // Alíq. IPI
                        }
                    });

                    table.Header(header =>
                    {
                        Header(header.Cell(), "CÓDIGO PRODUTO", isFirst: true);
                        Header(header.Cell(), "DESCRIÇÃO DO PRODUTO/SERVIÇO", drawLeftDivider: true);
                        Header(header.Cell(), "NCM/SH", alignCenter: true);
                        Header(header.Cell(), "CST", alignCenter: true);
                        Header(header.Cell(), "CFOP", alignCenter: true);
                        Header(header.Cell(), "UN", alignCenter: true);
                        Header(header.Cell(), "QUANT", alignCenter: true);
                        Header(header.Cell(), "VALOR\nUNIT", alignRight: true);
                        Header(header.Cell(), "VALOR\nTOTAL", alignRight: true);
                        Header(header.Cell(), "VALOR\nDESC", alignRight: true);
                        Header(header.Cell(), "BC ICMS", alignRight: true, drawLeftDivider: true);
                        Header(header.Cell(), "VALOR\nICMS", alignRight: true);
                        Header(header.Cell(), "VALOR\nIPI", alignRight: true);
                        Header(header.Cell(), "ALÍQ. ICMS", alignRight: true);
                        Header(header.Cell(), "ALÍQ. IPI", alignRight: true);
                    });

                    foreach (ProdutoModel prod in _produtos)
                    {
                        Data(table.Cell(), prod.Codigo, isFirst: true);
                        Data(table.Cell(), prod.Descricao, drawLeftDivider: true);
                        Data(table.Cell(), prod.Ncm, alignCenter: true);
                        Data(table.Cell(), prod.CstCsosn, alignCenter: true);
                        Data(table.Cell(), prod.Cfop, alignCenter: true);
                        Data(table.Cell(), prod.Unidade, alignCenter: true);
                        Data(table.Cell(), FormatQuantity(prod.Quantidade), alignRight: true);
                        Data(table.Cell(), FormatPrice(prod.ValorUnitario), alignRight: true);
                        Data(table.Cell(), DocumentFormatter.Money(prod.ValorTotal), alignRight: true);
                        Data(table.Cell(), DocumentFormatter.Money(prod.ValorDesconto), alignRight: true);
                        Data(table.Cell(), DocumentFormatter.Money(prod.BaseCalculoIcms), alignRight: true, drawLeftDivider: true);
                        Data(table.Cell(), DocumentFormatter.Money(prod.ValorIcms), alignRight: true);
                        Data(table.Cell(), DocumentFormatter.Money(prod.ValorIpi), alignRight: true);
                        Data(table.Cell(), FormatPercentage(prod.AliquotaIcms), alignRight: true);
                        Data(table.Cell(), FormatPercentage(prod.AliquotaIpi), alignRight: true);
                    }
                });
        });
    }

    private static void Header(IContainer container, string text, bool isFirst = false, bool alignRight = false, bool alignCenter = false, bool drawLeftDivider = false)
    {
        Cell(container, text, isHeader: true, isFirstColumn: isFirst, alignRight: alignRight, alignCenter: alignCenter, drawLeftDivider: drawLeftDivider);
    }

    private static void Data(IContainer container, string text, bool isFirst = false, bool alignRight = false, bool alignCenter = false, bool drawLeftDivider = false)
    {
        Cell(container, text, isHeader: false, isFirstColumn: isFirst, alignRight: alignRight, alignCenter: alignCenter, drawLeftDivider: drawLeftDivider);
    }

    private static void Cell(IContainer container, string text, bool isHeader, bool isFirstColumn, bool alignRight = false, bool alignCenter = false, bool drawLeftDivider = false)
    {
        container.Layers(layers =>
        {
            layers.PrimaryLayer().Element(content =>
            {
                IContainer cell = content
                    .Background(isHeader ? DanfeTheme.CorFundoTabela : Colors.White)
                    .PaddingHorizontal(2)
                    .PaddingVertical(1)
                    .AlignMiddle();

                if (alignRight)
                {
                    cell = cell.AlignRight();
                }
                else if (alignCenter)
                {
                    cell = cell.AlignCenter();
                }

                cell.Text(text)
                    .FontFamily(DanfeTheme.FontePadrao)
                    .FontSize(isHeader ? 4.2f : 5f)
                    .Style(isHeader ? TextStyle.Default.Bold() : TextStyle.Default);
            });

            if (isHeader)
            {
                layers.Layer().AlignTop().LineHorizontal(InternalGridThickness).LineColor(Colors.Grey.Lighten1).LineDashPattern(DashedGridPattern);
            }

            if (drawLeftDivider)
            {
                layers.Layer().AlignLeft().LineVertical(InternalGridThickness).LineColor(Colors.Grey.Lighten1).LineDashPattern(DashedGridPattern);
            }

            layers.Layer().AlignBottom().LineHorizontal(InternalGridThickness).LineColor(Colors.Grey.Lighten1).LineDashPattern(DashedGridPattern);
            layers.Layer().AlignRight().LineVertical(InternalGridThickness).LineColor(Colors.Grey.Lighten1).LineDashPattern(DashedGridPattern);
        });
    }

    private static string FormatQuantity(decimal value)
    {
        return value.ToString("N4", DocumentFormatter.Brazil);
    }

    private static string FormatPrice(decimal value)
    {
        return value % 0.01m != 0
            ? value.ToString("N4", DocumentFormatter.Brazil)
            : value.ToString("N2", DocumentFormatter.Brazil);
    }

    private static string FormatPercentage(decimal value)
    {
        return value.ToString("N2", DocumentFormatter.Brazil);
    }
}

public static class ProdutosBoxExtensions
{
    public static void ProdutosBox(this IContainer container, IReadOnlyList<ProdutoModel>? produtos, bool isLandscape = false)
    {
        container.Component(new ProdutosBox(produtos, isLandscape));
    }
}
